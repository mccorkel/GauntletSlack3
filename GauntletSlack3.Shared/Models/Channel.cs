using System.Collections.Generic;

namespace GauntletSlack3.Shared.Models;

public class Channel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<ChannelUser> ChannelUsers { get; set; }
} 