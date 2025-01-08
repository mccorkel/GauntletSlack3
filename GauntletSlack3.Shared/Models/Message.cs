namespace GauntletSlack3.Shared.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int ChannelId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Channel? Channel { get; set; }
} 