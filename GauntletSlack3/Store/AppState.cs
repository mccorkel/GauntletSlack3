using GauntletSlack3.Services.Interfaces;
using GauntletSlack3.Shared.Models;

public class AppState
{
    private readonly IMessageService _messageService;
    private readonly IChannelService _channelService;
    
    private Dictionary<int, List<Message>> _channelMessages = new();
    
    public event Action? OnChange;
    
    public async Task LoadChannelMessagesAsync(int channelId)
    {
        var messages = await _messageService.GetChannelMessagesAsync(channelId);
        _channelMessages[channelId] = messages;
        OnChange?.Invoke();
    }
} 