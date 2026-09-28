using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace LibraryManagement.Application.Features.Dashboard.Queries.GetDashboard;
public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IApplicationDbContext _context;
    public GetDashboardQueryHandler(IApplicationDbContext context) => _context = context;
    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return new DashboardDto(
            await _context.Users.CountAsync(ct),
            await _context.Books.CountAsync(ct),
            await _context.Books.SumAsync(x => (int?)x.AvailableCopies, ct) ?? 0,
            await _context.BorrowRecords.CountAsync(x => x.ReturnDate == null, ct),
            await _context.BorrowRecords.CountAsync(x => x.ReturnDate == null && x.DueDate < now, ct),
            await _context.Penalties.CountAsync(x => x.PaymentStatus == "Unpaid", ct),
            await _context.Penalties.Where(x => x.PaymentStatus == "Unpaid")
                .SumAsync(x => (decimal?)x.FineAmount, ct) ?? 0m);
    }
}
