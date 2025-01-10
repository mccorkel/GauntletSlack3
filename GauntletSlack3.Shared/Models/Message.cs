namespace GauntletSlack3.Shared.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int ChannelId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ParentMessageId { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Channel? Channel { get; set; }
    public Message? ParentMessage { get; set; }
    public List<Message> Replies { get; set; } = new();
    public List<MessageReaction> Reactions { get; set; } = new();
}

public class MessageReaction
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Message? Message { get; set; }
    public User? User { get; set; }
} 