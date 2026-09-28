using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.BorrowRecords.Commands.BorrowBook;

public class BorrowBookCommandHandler
    : IRequestHandler<BorrowBookCommand, BorrowRecordDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITransactionManager _transactions;
    private readonly IRowLockService _locks;

    public BorrowBookCommandHandler(
        IApplicationDbContext context,
        ITransactionManager transactions,
        IRowLockService locks)
    {
        _context = context;
        _transactions = transactions;
        _locks = locks;
    }

    public async Task<BorrowRecordDto> Handle(
        BorrowBookCommand request,
        CancellationToken ct)
    {
        await using var transaction =
            await _transactions.BeginAsync(ct);

        // 1. Lock reader first
        var reader = await _locks.LockUserAsync(
            request.ReaderId, ct);

        if (reader == null || !reader.IsActive)
            throw new InvalidOperationException(
                "Reader is not active.");

        // 2. Lock book
        var book = await _locks.LockBookAsync(
            request.BookId, ct);

        if (book == null)
            throw new KeyNotFoundException(
                "Book not found.");

        if (book.IsDeleted)
            throw new InvalidOperationException(
                "Book is no longer available.");

        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException(
                "Book is out of stock.");

        // 3. Check borrowing conditions
        var now = DateTime.UtcNow;

        var activeRecords = await _context.BorrowRecords
            .AsNoTracking()
            .Where(x =>
                x.ReaderId == request.ReaderId &&
                x.ReturnDate == null)
            .Select(x => new
            {
                x.DueDate
            })
            .ToListAsync(ct);

        if (activeRecords.Count >= 5)
            throw new InvalidOperationException(
                "Borrowing limit reached.");

        if (activeRecords.Any(x => x.DueDate < now))
            throw new InvalidOperationException(
                "Reader has overdue books.");

        var hasUnpaidPenalty = await _context.Penalties
            .AsNoTracking()
            .AnyAsync(
                x => x.BorrowRecord.ReaderId == request.ReaderId &&
                     x.PaymentStatus == "Unpaid",
                ct);

        if (hasUnpaidPenalty)
            throw new InvalidOperationException(
                "Reader has unpaid penalties.");

        // 4. Create borrow record
        var record = new BorrowRecord
        {
            ReaderId = request.ReaderId,
            BookId = book.Id,
            BorrowDate = now,
            DueDate = now.AddDays(14),
            Status = "Borrowing"
        };

        // 5. Update stock
        book.AvailableCopies--;
        book.Version++;

        _context.BorrowRecords.Add(record);

        await _context.SaveChangesAsync(ct);

        // 6. Commit transaction
        await transaction.CommitAsync(ct);

        return new BorrowRecordDto(
            record.Id,
            book.Id,
            book.Title,
            request.ReaderId,
            record.BorrowDate,
            record.DueDate,
            record.ReturnDate,
            record.Status
        );
    }
}
