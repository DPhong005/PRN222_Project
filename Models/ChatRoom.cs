using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class ChatRoom
{
    public int RoomId { get; set; }

    public int UserOneId { get; set; }

    public int UserTwoId { get; set; }

    public int? ApplicationId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public virtual UserAccount UserOne { get; set; } = null!;

    public virtual UserAccount UserTwo { get; set; } = null!;

    public virtual Application? Application { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
