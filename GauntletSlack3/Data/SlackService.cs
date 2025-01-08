using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using GauntletSlack3.Shared.Models;

public class SlackService
{
    private readonly HttpClient _httpClient;

    public SlackService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Channel>> GetChannelsForUserAsync(string userId)
    {
        return await _httpClient.GetFromJsonAsync<List<Channel>>($"api/channels?userId={userId}") ?? new List<Channel>();
    }

    public async Task<List<Message>> GetChannelMessagesAsync(int channelId)
    {
        return await _httpClient.GetFromJsonAsync<List<Message>>($"api/channels/{channelId}/messages") ?? new List<Message>();
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        var response = await _httpClient.PostAsJsonAsync("api/messages", message);
        return await response.Content.ReadFromJsonAsync<Message>() ?? message;
    }

    public async Task<bool> JoinChannelAsync(string userId, string userName, int channelId)
    {
        var channelUser = new ChannelUser
        {
            ChannelId = channelId,
            UserId = userId
        };

        var response = await _httpClient.PostAsJsonAsync($"api/channels/{channelId}/join", channelUser);
        return response.IsSuccessStatusCode;
    }
} 