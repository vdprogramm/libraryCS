using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Features.Users.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler
    : IRequestHandler<ChangeUserRoleCommand, AdminUserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITransactionManager _transactions;
    private readonly IRowLockService _locks;

    public ChangeUserRoleCommandHandler(
        IApplicationDbContext context,
        ITransactionManager transactions,
        IRowLockService locks)
    {
        _context = context;
        _transactions = transactions;
        _locks = locks;
    }

    public async Task<AdminUserDto> Handle(
        ChangeUserRoleCommand request,
        CancellationToken ct)
    {
        var roleName = request.Role?.Trim().ToUpperInvariant();

        if (roleName is not ("READER" or "LIBRARIAN"))
            throw new ArgumentException(
                "Role must be READER or LIBRARIAN.");

        await using var transaction =
            await _transactions.BeginAsync(ct);

        var user = await _locks.LockUserAsync(
            request.UserId, ct);

        if (user == null)
            throw new NotFoundException("User not found.");

        if (!user.IsActive)
            throw new BusinessException(
                "Cannot change the role of an inactive user.");

        // Load the user's current roles.
        await _context.UserRoles
            .Where(x => x.UserId == user.Id)
            .LoadAsync(ct);

        var currentRoles = await _context.UserRoles
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .ToListAsync(ct);

        if (currentRoles.Contains("ADMIN"))
            throw new BusinessException(
                "Admin roles cannot be changed here.");

        var role = await _context.Roles
            .SingleOrDefaultAsync(
                x => x.Name == roleName, ct);

        if (role == null)
            throw new NotFoundException(
                "Target role has not been initialized.");

        var existing = await _context.UserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync(ct);

        _context.UserRoles.RemoveRange(existing);

        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return new AdminUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.IsActive,
            new List<string> { role.Name }
        );
    }
}
