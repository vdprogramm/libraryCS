using System;

namespace LibraryManagement.Application.Features.Penalties.DTOs;

public record PenaltyDto(
    Guid Id,
    Guid BorrowRecordId,
    Guid ReaderId,
    string BookTitle,
    decimal FineAmount,
    string PaymentStatus,
    string Reason,
    DateTime? PaidAt
);
