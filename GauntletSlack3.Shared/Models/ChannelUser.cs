namespace GauntletSlack3.Shared.Models;

public class ChannelUser
{
    public int ChannelId { get; set; }
    public int UserId { get; set; }
    public virtual Channel? Channel { get; set; }
} 