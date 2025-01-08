namespace GauntletSlack3.Shared.Models;

public class ChannelUser
{
    public int ChannelId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public virtual Channel? Channel { get; set; }
} 