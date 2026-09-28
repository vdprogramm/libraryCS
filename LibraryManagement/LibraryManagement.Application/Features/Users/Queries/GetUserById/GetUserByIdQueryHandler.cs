using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Users.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, AdminUserDto?>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUserDto?> Handle(
        GetUserByIdQuery request,
        CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new AdminUserDto(
                x.Id,
                x.Username,
                x.Email,
                x.FullName,
                x.IsActive,
                x.UserRoles
                    .Select(ur => ur.Role.Name)
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);
    }
}
