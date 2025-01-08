using System.Collections.Generic;

namespace GauntletSlack3.Shared.Models;

public class Channel
{
    public Channel()
    {
        Messages = new List<Message>();
        Memberships = new List<ChannelMembership>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public virtual User? Owner { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<ChannelMembership> Memberships { get; set; }
} 