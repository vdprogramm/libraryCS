using System;

namespace LibraryManagement.Domain.Entities;

public class BorrowRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReaderId { get; set; }
    public User Reader { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = "Borrowing";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ReturnedById { get; set; }
    public User? ReturnedBy { get; set; }

    public Penalty? Penalty { get; set; }
}
