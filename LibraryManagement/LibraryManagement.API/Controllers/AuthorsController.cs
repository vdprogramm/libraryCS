using LibraryManagement.Application.Features.Authors.Commands.CreateAuthor;
using LibraryManagement.Application.Features.Authors.Queries.GetAuthors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly ISender _sender;

    public AuthorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAuthorsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAuthorCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(
                command,
                cancellationToken);

            return Created(
                $"/api/authors/{result.Id}",
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
