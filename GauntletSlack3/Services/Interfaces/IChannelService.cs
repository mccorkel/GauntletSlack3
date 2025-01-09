using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services.Interfaces
{
    public interface IChannelService
    {
        Task<List<Channel>> GetUserChannelsAsync(int userId);
    }
} 