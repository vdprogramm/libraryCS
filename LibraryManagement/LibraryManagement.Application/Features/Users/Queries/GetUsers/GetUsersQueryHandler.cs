using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Users.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, PagedUsersResult>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedUsersResult> Handle(
        GetUsersQuery request,
        CancellationToken ct)
    {
        var page = Math.Max(1, request.PageNumber);
        var size = Math.Clamp(request.PageSize, 1, 100);

        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim().ToLower();

            query = query.Where(x =>
                x.Username.ToLower().Contains(keyword) ||
                x.Email.ToLower().Contains(keyword) ||
                x.FullName.ToLower().Contains(keyword));
        }

        var total = await query.CountAsync(ct);

        var users = await query
            .OrderBy(x => x.Username)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
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
            .ToListAsync(ct);

        return new PagedUsersResult(
            users, page, size, total);
    }
}
