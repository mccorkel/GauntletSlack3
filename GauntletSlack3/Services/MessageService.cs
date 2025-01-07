using System.Net.Http.Json;
using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _http;

        public MessageService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Message>> GetChannelMessages(int channelId)
        {
            return await _http.GetFromJsonAsync<List<Message>>($"api/messages/{channelId}") ?? new List<Message>();
        }

        public async Task<Message> SaveMessage(Message message)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/messages", message);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {error}");
                    throw new Exception($"API returned {response.StatusCode}: {error}");
                }

                return await response.Content.ReadFromJsonAsync<Message>() 
                    ?? throw new Exception("Null response from API");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveMessage: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                var response = await _http.GetAsync("api/messages/test");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
} 