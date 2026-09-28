using System;
using LibraryManagement.Application.Features.Users.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Users.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(
    Guid UserId,
    string Role
) : IRequest<AdminUserDto>;
