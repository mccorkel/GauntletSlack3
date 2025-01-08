using GauntletSlack3.Shared.Models;

namespace GauntletSlack3.Services;

public interface IMessageService
{
    Task<List<Channel>> GetChannels();
    Task<Channel> SaveChannel(Channel channel);
    Task<List<Message>> GetChannelMessages(int channelId);
    Task<Message> SaveMessage(int channelId, Message message);
} 