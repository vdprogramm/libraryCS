using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Features.Users.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Users.Commands.ChangeUserStatus;

public class ChangeUserStatusCommandHandler
    : IRequestHandler<ChangeUserStatusCommand, AdminUserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITransactionManager _transactions;
    private readonly IRowLockService _locks;

    public ChangeUserStatusCommandHandler(
        IApplicationDbContext context,
        ITransactionManager transactions,
        IRowLockService locks)
    {
        _context = context;
        _transactions = transactions;
        _locks = locks;
    }

    public async Task<AdminUserDto> Handle(
        ChangeUserStatusCommand request,
        CancellationToken ct)
    {
        await using var transaction =
            await _transactions.BeginAsync(ct);

        var user = await _locks.LockUserAsync(
            request.UserId, ct);

        if (user == null)
            throw new NotFoundException("User not found.");

        var roles = await _context.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .ToListAsync(ct);

        if (roles.Contains("ADMIN"))
            throw new BusinessException(
                "Admin accounts cannot be disabled or enabled here.");

        user.IsActive = request.IsActive;

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new AdminUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.IsActive,
            roles
        );
    }
}
