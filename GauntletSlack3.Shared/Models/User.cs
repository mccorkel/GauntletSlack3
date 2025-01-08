namespace GauntletSlack3.Shared.Models
{
    public class User
    {
        public User()
        {
            Messages = new List<Message>();
            ChannelMemberships = new List<ChannelMembership>();
            OwnedChannels = new List<Channel>();
        }

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<ChannelMembership>? ChannelMemberships { get; set; }
        public virtual ICollection<Channel>? OwnedChannels { get; set; }
    }
} 