namespace GauntletSlack3.Services.Interfaces;

public interface IUserStateService
{
    int? CurrentUserId { get; set; }
    string? CurrentUserName { get; set; }
    event Action OnUserStateChanged;
    Task InitializeAsync(string email, string name);
    void Clear();
} 