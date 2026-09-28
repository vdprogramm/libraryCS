using System;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.BorrowRecords.Commands.BorrowBook;

public record BorrowBookCommand(
    Guid ReaderId,
    Guid BookId
) : IRequest<BorrowRecordDto>;
