namespace GauntletSlack3.Shared.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsAdmin { get; set; }
    public virtual ICollection<ChannelMembership> ChannelMemberships { get; set; } = new List<ChannelMembership>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();
} 