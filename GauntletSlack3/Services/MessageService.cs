using GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;

namespace GauntletSlack3.Services
{
    public class MessageService : IMessageService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            ILocalStorageService localStorage, 
            HttpClient httpClient,
            ILogger<MessageService> logger)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Message>> GetChannelMessagesAsync(int channelId)
        {
            string cacheKey = $"channel_messages_{channelId}";
            var cachedMessages = await _localStorage.GetItemAsync<List<Message>>(cacheKey);
            if (cachedMessages != null)
            {
                return cachedMessages;
            }

            try
            {
                var messages = await _httpClient.GetFromJsonAsync<List<Message>>($"api/messages/channel/{channelId}");
                await _localStorage.SetItemAsync(cacheKey, messages);
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
                _logger.LogInformation("Sending message to channel {ChannelId} from user {UserId} with content: {Content}", 
                    channelId, userId, content);

                var response = await _httpClient.PostAsJsonAsync($"api/messages/channel/{channelId}", new
                {
                    UserId = userId,
                    Content = content
                });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Message>()
                        ?? throw new Exception("Failed to deserialize message response");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send message. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                throw new Exception($"Failed to send message: {errorContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to channel {ChannelId}", channelId);
                throw;
            }
        }

        public async Task<Message?> GetMessageAsync(int messageId)
        {
            try
            {
                _logger.LogInformation("Fetching message {MessageId}", messageId);
                return await _httpClient.GetFromJsonAsync<Message>($"api/messages/{messageId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching message {MessageId}", messageId);
                return null;
            }
        }

        public async Task<Message> SendReplyAsync(int parentMessageId, int userId, string content)
        {
            try
            {
                _logger.LogInformation("Sending reply to message {MessageId}", parentMessageId);
                var response = await _httpClient.PostAsJsonAsync($"api/messages/{parentMessageId}/reply", new
                {
                    UserId = userId,
                    Content = content
                });

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Message>()
                    ?? throw new Exception("Failed to deserialize reply");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reply to message {MessageId}", parentMessageId);
                throw;
            }
        }

        public async Task AddReactionAsync(int messageId, int userId, string emoji)
        {
            try
            {
                _logger.LogInformation("Adding reaction to message {MessageId}", messageId);
                var response = await _httpClient.PostAsJsonAsync($"api/messages/{messageId}/reactions", new
                {
                    UserId = userId,
                    Emoji = emoji
                });
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding reaction to message {MessageId}", messageId);
                throw;
            }
        }

        public async Task RemoveReactionAsync(int messageId, int userId, string emoji)
        {
            try
            {
                _logger.LogInformation("Removing reaction from message {MessageId}", messageId);
                var response = await _httpClient.DeleteAsync(
                    $"api/messages/{messageId}/reactions?userId={userId}&emoji={Uri.EscapeDataString(emoji)}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing reaction from message {MessageId}", messageId);
                throw;
            }
        }
    }
}