using System;

namespace LibraryManagement.Application.Features.BorrowRecords.DTOs;

public record BorrowRecordDto(
    Guid Id,
    Guid BookId,
    string BookTitle,
    Guid ReaderId,
    DateTime BorrowDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    string Status
);
