using System;
using LibraryManagement.Application.Features.Users.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Users.Commands.ChangeUserStatus;

public record ChangeUserStatusCommand(
    Guid UserId,
    bool IsActive
) : IRequest<AdminUserDto>;
