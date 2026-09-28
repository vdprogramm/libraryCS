using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Penalties.Commands.PayPenalty;
using LibraryManagement.Application.Features.Penalties.Queries.GetMyPenalties;
using LibraryManagement.Application.Features.Penalties.Queries.GetPenalties;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/penalties")]
[Authorize]
public class PenaltiesController : ControllerBase
{
    private readonly ISender _sender;

    public PenaltiesController(ISender sender)
    {
        _sender = sender;
    }

    // GET /api/penalties/my
    [HttpGet("my")]
    [Authorize(Roles = "READER")]
    public async Task<IActionResult> GetMyPenalties(
        CancellationToken ct)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var result = await _sender.Send(
            new GetMyPenaltiesQuery(userId), ct);

        return Ok(result);
    }

    // GET /api/penalties?paymentStatus=Unpaid
    [HttpGet]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? paymentStatus,
        CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(
                new GetPenaltiesQuery(paymentStatus), ct);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/penalties/{id}/pay
    [HttpPut("{id:guid}/pay")]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> Pay(
        Guid id,
        CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(
                new PayPenaltyCommand(id), ct);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
