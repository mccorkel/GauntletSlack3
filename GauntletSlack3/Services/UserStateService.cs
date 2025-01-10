using GauntletSlack3.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GauntletSlack3.Services
{
    public class UserStateService : IUserStateService, IAsyncDisposable
    {
        private readonly ILogger<UserStateService> _logger;
        private readonly RealTimeService _realTimeService;
        private readonly IUserService _userService;
        private Dictionary<int, bool> _userStatuses = new();
        private int? _currentUserId;
        public int? CurrentUserId => _currentUserId;
        public event EventHandler? OnUserStatusChanged;
        private bool _isInitialized;

        public UserStateService(
            ILogger<UserStateService> logger, 
            RealTimeService realTimeService,
            IUserService userService)
        {
            _logger = logger;
            _realTimeService = realTimeService;
            _userService = userService;
            _realTimeService.OnUserStatusChanged += HandleUserStatusChanged;
            _realTimeService.OnUserJoined += HandleUserJoined;
            _realTimeService.OnReconnected += HandleReconnected;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            await InitializeUserStatuses();
            _isInitialized = true;
        }

        private void HandleUserStatusChanged(object? sender, UserStatusChangedEventArgs e)
        {
            _userStatuses[e.UserId] = e.IsOnline;
            OnUserStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsUserOnline(int userId)
        {
            return _userStatuses.TryGetValue(userId, out bool status) && status;
        }

        public async Task SetUserOnlineStatus(int userId, bool isOnline)
        {
            try
            {
                _userStatuses[userId] = isOnline;
                await _realTimeService.UpdateUserStatusAsync(userId, isOnline);
                OnUserStatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting user online status");
                throw;
            }
        }

        private async Task HandleReconnected()
        {
            try
            {
                // Refresh all user statuses after reconnection
                var users = await _userService.GetUsersAsync();
                foreach (var user in users)
                {
                    _userStatuses[user.Id] = user.IsOnline;
                }
                OnUserStatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing user statuses after reconnection");
            }
        }

        private async Task InitializeUserStatuses()
        {
            try
            {
                _currentUserId = await _userService.GetCurrentUserId();
                var users = await _userService.GetUsersAsync();
                foreach (var user in users)
                {
                    _userStatuses[user.Id] = user.IsOnline;
                }
                OnUserStatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing user statuses");
            }
        }

        private async void HandleUserJoined(object? sender, int userId)
        {
            try
            {
                // Get the new user's status and add them to our tracking
                var users = await _userService.GetUsersAsync();
                var newUser = users.FirstOrDefault(u => u.Id == userId);
                if (newUser != null)
                {
                    _userStatuses[userId] = newUser.IsOnline;
                    OnUserStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling new user joined");
            }
        }

        public async ValueTask DisposeAsync()
        {
            _realTimeService.OnUserStatusChanged -= HandleUserStatusChanged;
            _realTimeService.OnUserJoined -= HandleUserJoined;
            _realTimeService.OnReconnected -= HandleReconnected;
            
            // Clear statuses
            _userStatuses.Clear();
        }
    }
} 