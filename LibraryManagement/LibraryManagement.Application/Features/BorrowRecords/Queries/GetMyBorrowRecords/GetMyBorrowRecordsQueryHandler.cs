using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.BorrowRecords.Queries.GetMyBorrowRecords;

public class GetMyBorrowRecordsQueryHandler
    : IRequestHandler<GetMyBorrowRecordsQuery,
        List<BorrowRecordDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyBorrowRecordsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BorrowRecordDto>> Handle(
        GetMyBorrowRecordsQuery request,
        CancellationToken ct)
    {
        return await _context.BorrowRecords
            .AsNoTracking()
            .Where(x => x.ReaderId == request.ReaderId)
            .OrderByDescending(x => x.BorrowDate)
            .Select(x => new BorrowRecordDto(
                x.Id,
                x.BookId,
                x.Book.Title,
                x.ReaderId,
                x.BorrowDate,
                x.DueDate,
                x.ReturnDate,
                x.Status
            ))
            .ToListAsync(ct);
    }
}
