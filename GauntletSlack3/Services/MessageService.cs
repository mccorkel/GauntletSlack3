using GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;
using System.Net.Http.Json;

namespace GauntletSlack3.Services
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _http;
        private readonly ILogger<MessageService> _logger;

        public MessageService(HttpClient http, ILogger<MessageService> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<List<Message>> GetChannelMessagesAsync(int channelId)
        {
            try
            {
                var messages = await _http.GetFromJsonAsync<List<Message>>($"api/messages/channel/{channelId}");
                return messages ?? new List<Message>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching messages for channel {ChannelId}", channelId);
                throw;
            }
        }

        public async Task<Message> SendMessageAsync(int channelId, int userId, string content)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/messages", new 
                { 
                    ChannelId = channelId,
                    UserId = userId,
                    Content = content
                });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Message>() 
                        ?? throw new Exception("Failed to deserialize message response");
                }

                _logger.LogError("Failed to send message. Status: {StatusCode}", response.StatusCode);
                throw new Exception("Failed to send message");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to channel {ChannelId}", channelId);
                throw;
            }
        }
    }
}