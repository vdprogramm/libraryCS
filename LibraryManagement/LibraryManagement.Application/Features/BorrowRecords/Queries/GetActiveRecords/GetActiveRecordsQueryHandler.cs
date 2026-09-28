using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace LibraryManagement.Application.Features.BorrowRecords.Queries.GetActiveRecords;
public class GetActiveRecordsQueryHandler : IRequestHandler<GetActiveRecordsQuery, List<BorrowRecordDto>>
{
    private readonly IApplicationDbContext _context;
    public GetActiveRecordsQueryHandler(IApplicationDbContext context) => _context = context;
    public async Task<List<BorrowRecordDto>> Handle(GetActiveRecordsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return await _context.BorrowRecords.AsNoTracking()
            .Where(x => x.ReturnDate == null).OrderBy(x => x.DueDate)
            .Select(x => new BorrowRecordDto(x.Id, x.BookId, x.Book.Title,
                x.ReaderId, x.BorrowDate, x.DueDate, x.ReturnDate,
                x.DueDate < now ? "Overdue" : x.Status))
            .ToListAsync(ct);
    }
}
