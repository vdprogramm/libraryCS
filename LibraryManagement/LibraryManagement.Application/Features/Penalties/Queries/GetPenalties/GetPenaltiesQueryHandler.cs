using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Penalties.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Penalties.Queries.GetPenalties;

public class GetPenaltiesQueryHandler
    : IRequestHandler<GetPenaltiesQuery, List<PenaltyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPenaltiesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PenaltyDto>> Handle(
        GetPenaltiesQuery request,
        CancellationToken ct)
    {
        var query = _context.Penalties.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.PaymentStatus))
        {
            var status = request.PaymentStatus.Trim();

            if (status != "Paid" && status != "Unpaid")
                throw new ArgumentException(
                    "PaymentStatus must be Paid or Unpaid.");

            query = query.Where(
                x => x.PaymentStatus == status);
        }

        return await query
            .OrderByDescending(x =>
                x.BorrowRecord.ReturnDate)
            .Select(x => new PenaltyDto(
                x.Id,
                x.BorrowRecordId,
                x.BorrowRecord.ReaderId,
                x.BorrowRecord.Book.Title,
                x.FineAmount,
                x.PaymentStatus,
                x.Reason ?? string.Empty,
                x.PaidAt
            ))
            .ToListAsync(ct);
    }
}
