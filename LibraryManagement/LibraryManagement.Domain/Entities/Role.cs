using System;
using System.Collections.Generic;

namespace LibraryManagement.Domain.Entities;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();
}
