using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using LibraryManagement.Application.Features.Penalties;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.BorrowRecords.Commands.ReturnBook;

public class ReturnBookCommandHandler
    : IRequestHandler<ReturnBookCommand, BorrowRecordDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITransactionManager _transactions;
    private readonly IRowLockService _locks;

    public ReturnBookCommandHandler(
        IApplicationDbContext context,
        ITransactionManager transactions,
        IRowLockService locks)
    {
        _context = context;
        _transactions = transactions;
        _locks = locks;
    }

    public async Task<BorrowRecordDto> Handle(
        ReturnBookCommand request,
        CancellationToken ct)
    {
        await using var transaction =
            await _transactions.BeginAsync(ct);

        // Read IDs before locking
        var info = await _context.BorrowRecords
            .AsNoTracking()
            .Where(x => x.Id == request.BorrowRecordId)
            .Select(x => new
            {
                x.ReaderId,
                x.BookId
            })
            .FirstOrDefaultAsync(ct);

        if (info == null)
            throw new KeyNotFoundException(
                "Borrow record not found.");

        // Lock in consistent order
        var reader = await _locks.LockUserAsync(
            info.ReaderId, ct);

        if (reader == null)
            throw new KeyNotFoundException(
                "Reader not found.");

        // Librarian is authenticated by the API
        var book = await _locks.LockBookAsync(
            info.BookId, ct);

        if (book == null)
            throw new KeyNotFoundException(
                "Book not found.");

        var record = await _locks.LockBorrowRecordAsync(
            request.BorrowRecordId, ct);

        if (record == null)
            throw new KeyNotFoundException(
                "Borrow record not found.");

        if (record.ReturnDate != null)
            throw new InvalidOperationException(
                "Book has already been returned.");

        var now = DateTime.UtcNow;

        record.ReturnDate = now;
        record.ReturnedById = request.LibrarianId;

        var fineAmount = PenaltyCalculator.Calculate(
            record.DueDate,
            now);

        record.Status = fineAmount > 0
            ? "Overdue"
            : "Returned";

        // Create penalty in the same transaction
        if (fineAmount > 0)
        {
            var penalty = new Penalty
            {
                BorrowRecordId = record.Id,
                FineAmount = fineAmount,
                PaymentStatus = "Unpaid",
                Reason = "Overdue book return",
                PaidAt = null
            };

            _context.Penalties.Add(penalty);
        }

        book.AvailableCopies++;
        book.Version++;

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new BorrowRecordDto(
            record.Id,
            book.Id,
            book.Title,
            record.ReaderId,
            record.BorrowDate,
            record.DueDate,
            record.ReturnDate,
            record.Status
        );
    }
}
