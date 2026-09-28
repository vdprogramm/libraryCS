using LibraryManagement.Application.Features.Categories.Commands.CreateCategory;
using LibraryManagement.Application.Features.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoriesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(
                command,
                cancellationToken);

            return Created(
                $"/api/categories/{result.Id}",
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
