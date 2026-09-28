using LibraryManagement.Application.Features.Auth.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCurrentUserQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == request.UserId && x.IsActive)
            .Select(x => new UserDto(
                x.Id,
                x.Username,
                x.Email,
                x.FullName,
                x.UserRoles
                    .Select(r => r.Role.Name)
                    .ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
