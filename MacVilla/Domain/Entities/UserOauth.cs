using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class UserOauth
{
    public long OauthId { get; set; }

    public long UserId { get; set; }

    public string? Provider { get; set; }

    public string? ProviderUserId { get; set; }

    public virtual User User { get; set; } = null!;
}
