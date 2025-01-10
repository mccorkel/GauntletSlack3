using Azure.Core;
using Azure.Messaging.WebPubSub;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace GauntletSlack3.Services;

public class RealTimeService : IAsyncDisposable
{
    private readonly WebPubSubServiceClient _client;
    private readonly ILogger<RealTimeService> _logger;
    private bool _isConnected;

    public event EventHandler<UserStatusChangedEventArgs>? OnUserStatusChanged;

    public RealTimeService(IConfiguration config, ILogger<RealTimeService> logger)
    {
        _logger = logger;
        var connectionString = config.GetConnectionString("WebPubSub");
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("WebPubSub connection string is missing");
            return;
        }
        _logger.LogInformation("Initializing WebPubSub client with hub: users");
        _client = new WebPubSubServiceClient(connectionString, "users");
        _isConnected = true;
    }

    public async Task StartAsync()
    {
        try
        {
            _logger.LogInformation("Starting WebPubSub client...");
            _logger.LogInformation("Subscribing to users group...");
            await _client.AddUserToGroupAsync("users", "all");
            
            await _client.SendToGroupAsync("users", 
                BinaryData.FromString(JsonSerializer.Serialize(new
                {
                    type = "connection",
                    message = "Client connected"
                })),
                ContentType.ApplicationJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start WebPubSub client. Connection string might be invalid or service unavailable");
        }
    }

    public async Task UpdateUserStatusAsync(int userId, bool isOnline)
    {
        if (!_isConnected)
        {
            _logger.LogWarning("Cannot update user status: WebPubSub client is not connected");
            return;
        }
        
        try
        {
            _logger.LogInformation("Sending user status update: UserId={UserId}, IsOnline={IsOnline}", userId, isOnline);
            await _client.SendToGroupAsync("users", 
                BinaryData.FromString(JsonSerializer.Serialize(new
                {
                    type = "userStatus",
                    userId = userId,
                    isOnline = isOnline
                })),
                ContentType.ApplicationJson);
            _logger.LogDebug("User status update sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send user status update for UserId={UserId}", userId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("RealTimeService disposed");
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