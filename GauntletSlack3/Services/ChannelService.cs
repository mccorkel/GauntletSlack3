using GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace GauntletSlack3.Services;

public class ChannelService : IChannelService
{
    private readonly HttpClient _httpClient;
    private readonly IUserStateService _userStateService;
    private readonly ILogger<ChannelService> _logger;

    public event EventHandler? OnChannelMembershipChanged;

    public ChannelService(HttpClient httpClient, IUserStateService userStateService, ILogger<ChannelService> logger)
    {
        _httpClient = httpClient;
        _userStateService = userStateService;
        _logger = logger;
    }

    public async Task<List<Channel>> GetUserChannelsAsync(int userId)
    {
        Console.WriteLine($"Client: Requesting channels for user {userId}");
        var channels = await _httpClient.GetFromJsonAsync<List<Channel>>($"api/channels?userId={userId}");
        Console.WriteLine($"Client: Received {channels?.Count ?? 0} channels");
        return channels ?? new List<Channel>();
    }

    public async Task<Channel> CreateChannelAsync(string name, string type)
    {
        Console.WriteLine($"ChannelService: Creating channel {name} of type {type}");
        Console.WriteLine($"Current user ID: {_userStateService.CurrentUserId}");
        var request = new { Name = name, Type = type, UserId = _userStateService.CurrentUserId!.Value };
        var response = await _httpClient.PostAsJsonAsync("api/Channels", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Channel>() ?? 
            throw new Exception("Failed to create channel");
    }

    public async Task<List<User>> GetChannelMembersAsync(int channelId)
    {
        return await _httpClient.GetFromJsonAsync<List<User>>($"api/Channels/{channelId}/members") 
            ?? new List<User>();
    }

    public async Task JoinChannelAsync(int channelId, int userId)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/Channels/{channelId}/join", userId);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to join channel");
        }
    }

    public async Task LeaveChannelAsync(int channelId, int userId)
    {
        _logger.LogInformation($"Leaving channel {channelId} as user {userId}");
        var response = await _httpClient.PostAsJsonAsync($"api/channels/{channelId}/leave", userId);
        response.EnsureSuccessStatusCode();
        OnChannelMembershipChanged?.Invoke(this, EventArgs.Empty);
    }
} 