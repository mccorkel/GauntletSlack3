using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services.Interfaces
{
    public interface IChannelService
    {
        event EventHandler? OnChannelMembershipChanged;
        Task<List<Channel>> GetUserChannelsAsync(int userId);
        Task<Channel> CreateChannelAsync(string name, string type);
        Task<List<User>> GetChannelMembersAsync(int channelId);
        Task JoinChannelAsync(int channelId, int userId);
        Task LeaveChannelAsync(int channelId, int userId);
    }
} 