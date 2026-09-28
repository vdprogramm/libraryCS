using System;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Persistence.Transactions;

public class RowLockService : IRowLockService
{
    private readonly ApplicationDbContext _context;

    public RowLockService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> LockUserAsync(
        Guid userId,
        CancellationToken ct)
    {
        return _context.Users
            .FromSqlInterpolated(
                $"""
                SELECT * FROM "Users"
                WHERE "Id" = {userId}
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(ct);
    }

    public Task<Book?> LockBookAsync(
        Guid bookId,
        CancellationToken ct)
    {
        return _context.Books
            .FromSqlInterpolated(
                $"""
                SELECT * FROM "Books"
                WHERE "Id" = {bookId}
                FOR UPDATE
                """)
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(ct);
    }

    public Task<BorrowRecord?> LockBorrowRecordAsync(
        Guid recordId,
        CancellationToken ct)
    {
        return _context.BorrowRecords
            .FromSqlInterpolated(
                $"""
                SELECT * FROM "BorrowRecords"
                WHERE "Id" = {recordId}
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(ct);
    }

    public Task<Penalty?> LockPenaltyAsync(
        Guid penaltyId,
        CancellationToken ct)
    {
        return _context.Penalties
            .FromSqlInterpolated(
                $"""
                SELECT * FROM "Penalties"
                WHERE "Id" = {penaltyId}
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(ct);
    }
}
