using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<Message>> GetChannelMessagesAsync(int channelId);
        Task<Message> SendMessageAsync(int channelId, int userId, string content);
    }
} 