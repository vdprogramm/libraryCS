using System;
using System.Collections.Generic;

namespace LibraryManagement.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Book> Books { get; set; }
        = new List<Book>();
}
