using GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;
using System.Net.Http.Json;

namespace GauntletSlack3.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetOrCreateUserAsync(string email, string name)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Users/getorcreate", new { Email = email, Name = name });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            Console.WriteLine($"Failed to create user. Status: {response.StatusCode}");
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            throw new Exception("Failed to get or create user");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("api/users") ?? new List<User>();
        }
    }
} 