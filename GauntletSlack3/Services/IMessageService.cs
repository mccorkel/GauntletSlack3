using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services
{
    public interface IMessageService
    {
        Task<List<Message>> GetChannelMessages(int channelId);
        Task<Message> SaveMessage(Message message);
        Task<bool> TestConnection();
    }
} 