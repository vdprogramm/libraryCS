using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.BorrowRecords.Queries.GetOverdueRecords;

public class GetOverdueRecordsQueryHandler
    : IRequestHandler<GetOverdueRecordsQuery,
        List<BorrowRecordDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOverdueRecordsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BorrowRecordDto>> Handle(
        GetOverdueRecordsQuery request,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await _context.BorrowRecords
            .AsNoTracking()
            .Where(x =>
                x.ReturnDate == null &&
                x.DueDate < now)
            .OrderBy(x => x.DueDate)
            .Select(x => new BorrowRecordDto(
                x.Id,
                x.BookId,
                x.Book.Title,
                x.ReaderId,
                x.BorrowDate,
                x.DueDate,
                x.ReturnDate,
                "Overdue"
            ))
            .ToListAsync(ct);
    }
}
