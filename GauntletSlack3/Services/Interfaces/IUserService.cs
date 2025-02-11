using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services.Interfaces;

public interface IUserService
{
    Task<int> GetOrCreateUserAsync(string email, string name);
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserAsync(int userId);
    Task<bool> UpdateUserStatusAsync(int userId, bool isOnline);
    Task<int> GetCurrentUserId();
} 