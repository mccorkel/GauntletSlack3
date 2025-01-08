using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services
{
    public interface IMessageService
    {
        Task<Message> SaveMessage(int channelId, Message message);
        Task<List<Message>> GetChannelMessages(int channelId);
        Task<Channel> SaveChannel(Channel channel);
        Task<List<Channel>> GetChannels();
    }
} 