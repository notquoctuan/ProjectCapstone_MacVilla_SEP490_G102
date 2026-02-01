using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Notification
{
    public long NotificationId { get; set; }

    public long? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
