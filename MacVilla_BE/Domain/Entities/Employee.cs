using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Employee
{
    public long EmployeeId { get; set; }

    public long? UserId { get; set; }

    public string? Position { get; set; }

    public virtual User? User { get; set; }
}
