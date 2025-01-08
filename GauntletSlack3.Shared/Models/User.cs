namespace GauntletSlack3.Shared.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    // Navigation properties
    public virtual ICollection<Channel>? OwnedChannels { get; set; }
    public virtual ICollection<Message>? Messages { get; set; }
    public virtual ICollection<ChannelMembership>? Memberships { get; set; }
} 