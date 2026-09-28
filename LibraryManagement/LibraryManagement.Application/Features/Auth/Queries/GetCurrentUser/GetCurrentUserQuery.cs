using LibraryManagement.Application.Features.Auth.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(
    Guid UserId
) : IRequest<UserDto?>;
