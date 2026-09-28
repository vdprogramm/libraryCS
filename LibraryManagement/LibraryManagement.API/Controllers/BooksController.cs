using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Application.Features.Books.Commands.UpdateBook;
using LibraryManagement.Application.Features.Books.Commands.DeleteBook;
using LibraryManagement.Application.Features.Books.Queries.GetBooks;
using LibraryManagement.Application.Features.Books.Queries.GetBookById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ISender _sender;

    public BooksController(ISender sender)
    {
        _sender = sender;
    }

    // GET /api/books
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetBooksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            query, cancellationToken);

        return Ok(result);
    }

    // GET /api/books/search?keyword=clean
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] GetBooksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            query, cancellationToken);

        return Ok(result);
    }

    // GET /api/books/{id}
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetBookByIdQuery(id),
            cancellationToken);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    // POST /api/books
    [HttpPost]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(
                command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT /api/books/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateBookCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new
            {
                message = "Book ID mismatch."
            });

        try
        {
            var result = await _sender.Send(
                command, cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "Book was modified by another request. Please reload."
            });
        }
    }

    // DELETE /api/books/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(
                new DeleteBookCommand(id),
                cancellationToken);

            return deleted ? NoContent() : NotFound();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "Book was modified by another request."
            });
        }
    }
}
