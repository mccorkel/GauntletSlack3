using System.Net.Http.Json;
using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services;

public class MessageService : IMessageService
{
    private readonly HttpClient _http;

    public MessageService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Channel>> GetChannels()
    {
        try
        {
            var channels = await _http.GetFromJsonAsync<List<Channel>>("api/channels");
            return channels ?? new List<Channel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching channels: {ex.Message}");
            return new List<Channel>();
        }
    }

    public async Task<Channel> SaveChannel(Channel channel)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/channels", channel);
            response.EnsureSuccessStatusCode();
            var savedChannel = await response.Content.ReadFromJsonAsync<Channel>();
            return savedChannel ?? channel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving channel: {ex.Message}");
            return channel;
        }
    }

    public async Task<List<Message>> GetChannelMessages(int channelId)
    {
        try
        {
            var messages = await _http.GetFromJsonAsync<List<Message>>($"api/channels/{channelId}/messages");
            return messages ?? new List<Message>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching messages: {ex.Message}");
            return new List<Message>();
        }
    }

    public async Task<Message> SaveMessage(int channelId, Message message)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"api/channels/{channelId}/messages", message);
            response.EnsureSuccessStatusCode();
            var savedMessage = await response.Content.ReadFromJsonAsync<Message>();
            return savedMessage ?? message;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving message: {ex.Message}");
            return message;
        }
    }
} 