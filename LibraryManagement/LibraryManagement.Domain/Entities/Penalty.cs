using System;

namespace LibraryManagement.Domain.Entities;

public class Penalty
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BorrowRecordId { get; set; }

    public BorrowRecord BorrowRecord { get; set; } = null!;

    public decimal FineAmount { get; set; }

    public string PaymentStatus { get; set; } = "Unpaid";

    public string? Reason { get; set; }

    public DateTime? PaidAt { get; set; }
}
