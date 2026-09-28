using System.Collections.Generic;
using LibraryManagement.Application.Features.Users.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(
    string? Keyword,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedUsersResult>;

public record PagedUsersResult(
    List<AdminUserDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);
