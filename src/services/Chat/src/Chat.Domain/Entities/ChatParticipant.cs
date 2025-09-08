namespace QuickChat.Chat.Domain.Entities;

public class ChatParticipant : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
