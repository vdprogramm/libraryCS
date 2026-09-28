using System;
using LibraryManagement.Application.Features.Users.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id)
    : IRequest<AdminUserDto?>;
