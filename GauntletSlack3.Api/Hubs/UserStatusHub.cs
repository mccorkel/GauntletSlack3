using Microsoft.Azure.WebPubSub.AspNetCore;
using Microsoft.Azure.WebPubSub.Common;

namespace GauntletSlack3.Api.Hubs;

public class UserStatusHub : WebPubSubHub
{
    private readonly ILogger<UserStatusHub> _logger;

    public UserStatusHub(ILogger<UserStatusHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync(ConnectedEventRequest request)
    {
        _logger.LogInformation("Client connected: {ConnectionId}", request.ConnectionContext.ConnectionId);
        await base.OnConnectedAsync(request);
    }

    public override async Task OnDisconnectedAsync(DisconnectedEventRequest request)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", request.ConnectionContext.ConnectionId);
        await base.OnDisconnectedAsync(request);
    }
} 