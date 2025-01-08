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
            return await _http.GetFromJsonAsync<List<Message>>($"api/messages/channel/{channelId}") 
                ?? new List<Message>();
        }

        public async Task<Message> SaveMessage(int channelId, Message message)
        {
            try
            {
                var messageToSend = new Message
                {
                    Content = message.Content,
                    UserId = message.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                var response = await _http.PostAsJsonAsync($"api/messages/channel/{channelId}", messageToSend);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
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

        public async Task<List<Channel>> GetChannels()
        {
            return await _http.GetFromJsonAsync<List<Channel>>("api/channels") 
                ?? new List<Channel>();
        }

        public async Task<Channel> SaveChannel(Channel channel)
        {
            try
            {
                // Don't send the collections
                var channelToSend = new Channel
                {
                    Name = channel.Name,
                    Type = channel.Type
                };

                var response = await _http.PostAsJsonAsync("api/channels", channelToSend);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API returned {response.StatusCode}: {error}");
                }

                return await response.Content.ReadFromJsonAsync<Channel>() 
                    ?? throw new Exception("Null response from API");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveChannel: {ex.Message}");
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