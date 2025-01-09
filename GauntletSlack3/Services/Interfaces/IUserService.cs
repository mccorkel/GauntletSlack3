namespace GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;

public interface IUserService
{
    Task<int> GetOrCreateUserAsync(string email, string name);
    Task<List<User>> GetUsersAsync();
} 