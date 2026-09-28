using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Penalties.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Penalties.Queries.GetMyPenalties;

public class GetMyPenaltiesQueryHandler
    : IRequestHandler<GetMyPenaltiesQuery, List<PenaltyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyPenaltiesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PenaltyDto>> Handle(
        GetMyPenaltiesQuery request,
        CancellationToken ct)
    {
        return await _context.Penalties
            .AsNoTracking()
            .Where(x =>
                x.BorrowRecord.ReaderId == request.ReaderId)
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
