namespace GauntletSlack3.Shared.Models;

public class ChannelUser
{
    public int ChannelId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Channel? Channel { get; set; }
} 