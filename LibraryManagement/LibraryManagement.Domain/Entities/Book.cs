using System;
using System.Collections.Generic;

namespace LibraryManagement.Domain.Entities;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }

    public decimal Price { get; set; }

    public int Version { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<BorrowRecord> BorrowRecords { get; set; }
        = new List<BorrowRecord>();
}
