using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using GauntletSlack3.Services.Interfaces;

namespace GauntletSlack3.Services;

public class RealTimeService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly ILogger<RealTimeService> _logger;
    private bool _isConnected;
    private readonly IUserService _userService;
    private readonly HttpClient _httpClient;
    private const int MaxRetries = 3;
    private int _retryCount = 0;

    public event EventHandler<UserStatusChangedEventArgs>? OnUserStatusChanged;
    public event EventHandler<int>? OnUserJoined;
    public event Func<Task>? OnReconnected;
    public event Action<bool>? ConnectionChanged;

    public RealTimeService(IConfiguration config, ILogger<RealTimeService> logger, IUserService userService, HttpClient httpClient)
    {
        _logger = logger;
        _userService = userService;
        _httpClient = httpClient;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_httpClient.BaseAddress}hubs/userstatus")
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
    }

    public async Task StartAsync()
    {
        try
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{_httpClient.BaseAddress}hubs/userstatus")
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.On<string, bool>("UserStatusChanged", (userIdStr, isOnline) =>
                {
                    if (int.TryParse(userIdStr, out int userId))
                    {
                        OnUserStatusChanged?.Invoke(this, new UserStatusChangedEventArgs(userId, isOnline));
                    }
                });
            }

            if (_hubConnection.State != HubConnectionState.Connected)
            {
                await _hubConnection.StartAsync();
                _isConnected = true;
                ConnectionChanged?.Invoke(true);
                
                var userId = await _userService.GetCurrentUserId();
                await _hubConnection.SendAsync("OnConnected", userId.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to SignalR hub");
            _isConnected = false;
            ConnectionChanged?.Invoke(false);
            throw;
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