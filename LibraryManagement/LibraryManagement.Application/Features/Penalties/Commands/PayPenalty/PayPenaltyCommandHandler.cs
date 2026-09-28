using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Penalties.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Penalties.Commands.PayPenalty;

public class PayPenaltyCommandHandler
    : IRequestHandler<PayPenaltyCommand, PenaltyDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITransactionManager _transactions;

    private readonly IRowLockService _locks;

    public PayPenaltyCommandHandler(
        IApplicationDbContext context,
        ITransactionManager transactions,
        IRowLockService locks)
    {
        _context = context;
        _transactions = transactions;
        _locks = locks;
    }

    public async Task<PenaltyDto> Handle(
        PayPenaltyCommand request,
        CancellationToken ct)
    {
        await using var transaction =
            await _transactions.BeginAsync(ct);

        // Lock penalty to prevent double payment
        var penalty = await _locks.LockPenaltyAsync(request.PenaltyId, ct);

        if (penalty == null)
            throw new KeyNotFoundException(
                "Penalty not found.");

        if (penalty.PaymentStatus == "Paid")
            throw new InvalidOperationException(
                "Penalty has already been paid.");

        if (penalty.PaymentStatus != "Unpaid")
            throw new InvalidOperationException(
                "Invalid payment status.");

        var record = await _context.BorrowRecords
            .AsNoTracking()
            .Where(x => x.Id == penalty.BorrowRecordId)
            .Select(x => new
            {
                x.ReaderId,
                BookTitle = x.Book.Title
            })
            .SingleAsync(ct);

        penalty.PaymentStatus = "Paid";
        penalty.PaidAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new PenaltyDto(
            penalty.Id,
            penalty.BorrowRecordId,
            record.ReaderId,
            record.BookTitle,
            penalty.FineAmount,
            penalty.PaymentStatus,
            penalty.Reason ?? string.Empty,
            penalty.PaidAt
        );
    }
}
