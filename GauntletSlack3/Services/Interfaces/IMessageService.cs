using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<Message>> GetChannelMessagesAsync(int channelId);
        Task<Message> SendMessageAsync(int channelId, int userId, string content);
        Task<Message> SendReplyAsync(int parentMessageId, int userId, string content);
        Task<Message?> GetMessageAsync(int messageId);
        Task AddReactionAsync(int messageId, int userId, string emoji);
        Task RemoveReactionAsync(int messageId, int userId, string emoji);
    }
} 