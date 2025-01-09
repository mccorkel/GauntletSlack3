using GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;
using System.Net.Http.Json;

namespace GauntletSlack3.Services;

public class ChannelService : IChannelService
{
    private readonly HttpClient _httpClient;

    public ChannelService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Channel>> GetUserChannelsAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<List<Channel>>($"api/channels") ?? new List<Channel>();
    }
} 