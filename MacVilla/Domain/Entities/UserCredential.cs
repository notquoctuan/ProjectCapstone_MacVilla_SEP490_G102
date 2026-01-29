using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class UserCredential
{
    public long CredentialId { get; set; }

    public long UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
