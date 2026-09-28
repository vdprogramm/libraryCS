using System.Security.Claims;
using LibraryManagement.Application.Features.Auth.Commands.Login;
using LibraryManagement.Application.Features.Auth.Commands.Register;
using LibraryManagement.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(
            new GetCurrentUserQuery(userId));

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
