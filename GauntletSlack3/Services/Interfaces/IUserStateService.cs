namespace GauntletSlack3.Services.Interfaces;

public interface IUserStateService
{
    int? CurrentUserId { get; }
    bool IsUserOnline(int userId);
    Task SetUserOnlineStatus(int userId, bool isOnline);
    event EventHandler? OnUserStatusChanged;
    Task InitializeAsync();
} 