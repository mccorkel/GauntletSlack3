using Microsoft.AspNetCore.SignalR;

namespace GauntletSlack3.Api.Hubs
{
    public class UserStatusHub : Hub
    {
        private readonly ILogger<UserStatusHub> _logger;

        public UserStatusHub(ILogger<UserStatusHub> logger)
        {
            _logger = logger;
        }

        public async Task OnConnected(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            _logger.LogInformation("User {UserId} connected", userId);
            await Clients.Others.SendAsync("UserJoined", userId);
        }

        public async Task UpdateStatus(string userId, bool isOnline)
        {
            _logger.LogInformation("User {UserId} status changed to {IsOnline}", userId, isOnline);
            await Clients.Others.SendAsync("UserStatusChanged", userId, isOnline);
        }

        public async Task OnDisconnected(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            _logger.LogInformation("User {UserId} disconnected", userId);
            await Clients.All.SendAsync("UserStatusChanged", userId, false);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Connection {ConnectionId} disconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
} 