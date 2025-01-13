using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using GauntletSlack3.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.AspNetCore.Http.Connections;

namespace GauntletSlack3.Services;

public class RealTimeService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly ILogger<RealTimeService> _logger;
    private readonly NavigationManager _navigationManager;
    private bool _isConnected;
    private readonly IUserService _userService;
    private readonly HttpClient _httpClient;
    private const int MaxRetries = 3;
    private int _retryCount = 0;
    private bool _isStarted;
    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

    public event EventHandler<UserStatusChangedEventArgs>? OnUserStatusChanged;
    public event EventHandler<int>? OnUserJoined;
    public event Func<Task>? OnReconnected;
    public event Action<bool>? ConnectionChanged;

    public RealTimeService(NavigationManager navigationManager, ILogger<RealTimeService> logger, IUserService userService, HttpClient httpClient)
    {
        _navigationManager = navigationManager;
        _logger = logger;
        _userService = userService;
        _httpClient = httpClient;

        // Use the API base URL instead of the client URL
        var hubUrl = _httpClient.BaseAddress + "hubs/userstatus";
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.Transports = HttpTransportType.WebSockets | 
                                   HttpTransportType.ServerSentEvents | 
                                   HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, bool>("UserStatusChanged", (userIdStr, isOnline) =>
        {
            if (int.TryParse(userIdStr, out int userId))
            {
                OnUserStatusChanged?.Invoke(this, new UserStatusChangedEventArgs(userId, isOnline));
            }
        });

        _hubConnection.On<string>("UserJoined", (userIdStr) =>
        {
            if (int.TryParse(userIdStr, out int userId))
            {
                OnUserJoined?.Invoke(this, userId);
            }
        });

        _hubConnection.Closed += async (error) =>
        {
            _isConnected = false;
            ConnectionChanged?.Invoke(false);
            _logger.LogWarning(error, "SignalR connection closed");
            await Task.CompletedTask;
        };

        _hubConnection.Reconnected += async (connectionId) =>
        {
            _isConnected = true;
            ConnectionChanged?.Invoke(true);
            _logger.LogInformation("SignalR connection restored");
            if (OnReconnected != null)
            {
                await OnReconnected.Invoke();
            }
        };

        _hubConnection.Reconnecting += (error) =>
        {
            _isConnected = false;
            ConnectionChanged?.Invoke(false);
            _logger.LogWarning(error, "SignalR attempting to reconnect");
            return Task.CompletedTask;
        };

        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        _hubConnection.On<string>("ReceiveMessage", (message) =>
        {
            // Your message handling logic
        });

        // Add other handlers here
    }

    public async Task StartAsync()
    {
        try
        {
            await _connectionLock.WaitAsync();
            
            if (_isStarted)
            {
                _logger.LogInformation("Hub connection already started");
                return;
            }

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                _isStarted = true;
                _isConnected = true;
                _logger.LogInformation("SignalR hub connection started");
            }
        }
        catch (Exception ex)
        {
            _isConnected = false;
            _logger.LogError(ex, "Error connecting to SignalR hub");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task UpdateUserStatusAsync(int userId, bool isOnline)
    {
        if (!_isConnected)
        {
            _logger.LogWarning("Cannot update status: Not connected");
            return;
        }

        try
        {
            await _hubConnection?.SendAsync("UpdateStatus", userId.ToString(), isOnline);
            _logger.LogInformation("Sent status update for user {UserId}: {IsOnline}", userId, isOnline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user status");
            _isConnected = false;
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            try
            {
                await _hubConnection.DisposeAsync();
                _isConnected = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing SignalR connection");
            }
        }
    }
}

public class UserStatusChangedEventArgs : EventArgs
{
    public int UserId { get; }
    public bool IsOnline { get; }

    public UserStatusChangedEventArgs(int userId, bool isOnline)
    {
        UserId = userId;
        IsOnline = isOnline;
    }
} 