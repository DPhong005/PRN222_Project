using System;

namespace DevHub.Models;

public partial class ChatMessage
{
    public long MessageId { get; set; }

    public int RoomId { get; set; }

    public int SenderId { get; set; }

    public string MessageText { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual UserAccount Sender { get; set; } = null!;
}
