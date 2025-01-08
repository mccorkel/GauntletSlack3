namespace GauntletSlack3.Shared.Models
{
    public class ChannelMembership
    {
        public int ChannelId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsMuted { get; set; }

        public virtual Channel? Channel { get; set; }
        public virtual User? User { get; set; }
    }
} 