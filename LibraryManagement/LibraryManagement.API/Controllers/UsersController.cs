using System;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Users.Commands.ChangeUserRole;
using LibraryManagement.Application.Features.Users.Commands.ChangeUserStatus;
using LibraryManagement.Application.Features.Users.Queries.GetUserById;
using LibraryManagement.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "ADMIN")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetUsersQuery(
                keyword, pageNumber, pageSize),
            ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new GetUserByIdQuery(id), ct);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeRoleRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new ChangeUserRoleCommand(id, request.Role),
            ct);

        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new ChangeUserStatusCommand(
                id, request.IsActive),
            ct);

        return Ok(result);
    }
}

public record ChangeRoleRequest(string Role);

public record ChangeStatusRequest(bool IsActive);
