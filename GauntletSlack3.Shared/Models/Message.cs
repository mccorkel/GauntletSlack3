namespace GauntletSlack3.Shared.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ChannelId { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Channel? Channel { get; set; }
} 