using MediatR;
namespace LibraryManagement.Application.Features.Dashboard.Queries.GetDashboard;
public record GetDashboardQuery() : IRequest<DashboardDto>;
public record DashboardDto(int TotalUsers, int TotalBooks, int AvailableCopies,
    int ActiveBorrows, int OverdueBorrows, int UnpaidPenalties, decimal UnpaidFineAmount);
