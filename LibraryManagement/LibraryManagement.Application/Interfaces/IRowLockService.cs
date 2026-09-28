using System;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface IRowLockService
{
    Task<User?> LockUserAsync(
        Guid userId,
        CancellationToken ct);

    Task<Book?> LockBookAsync(
        Guid bookId,
        CancellationToken ct);

    Task<BorrowRecord?> LockBorrowRecordAsync(
        Guid recordId,
        CancellationToken ct);

    Task<Penalty?> LockPenaltyAsync(
        Guid penaltyId,
        CancellationToken ct);
}
