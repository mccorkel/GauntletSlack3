using System.Net.Http.Json;
using GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;
using Microsoft.Extensions.Logging;

namespace GauntletSlack3.Services;

public class UserService : IUserService
{
    private readonly HttpClient _http;
    private readonly ILogger<UserService> _logger;

    public UserService(HttpClient http, ILogger<UserService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<int> GetOrCreateUserAsync(string email, string name)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/Users/getorcreate", new { Email = email, Name = name });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            _logger.LogError("Failed to create user. Status: {StatusCode}", response.StatusCode);
            throw new Exception("Failed to get or create user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrCreateUserAsync for {Email}", email);
            throw;
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            var users = await _http.GetFromJsonAsync<List<User>>("api/users");
            return users ?? new List<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users");
            throw;
        }
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<User>($"api/users/{userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> UpdateUserStatusAsync(int userId, bool isOnline)
    {
        try
        {
            _logger.LogInformation("Updating status for user {UserId} to {Status}", userId, isOnline ? "online" : "offline");
            var response = await _http.PutAsJsonAsync($"api/users/{userId}/status", new { IsOnline = isOnline });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for user {UserId}", userId);
            return false;
        }
    }
} 