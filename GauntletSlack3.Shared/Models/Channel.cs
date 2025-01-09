using System.Collections.Generic;

namespace GauntletSlack3.Shared.Models;

public class Channel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User? Owner { get; set; }
    public virtual ICollection<Message>? Messages { get; set; }
    public virtual ICollection<ChannelMembership>? Memberships { get; set; }
} 