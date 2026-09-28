using System;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.BorrowRecords.Commands.ReturnBook;

public record ReturnBookCommand(
    Guid BorrowRecordId,
    Guid LibrarianId
) : IRequest<BorrowRecordDto>;
