using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class RolePermission
{
    public string Role { get; set; } = null!;

    public string Permission { get; set; } = null!;
}
