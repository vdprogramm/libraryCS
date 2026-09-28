using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.BorrowRecords.Commands.BorrowBook;
using LibraryManagement.Application.Features.BorrowRecords.Commands.ReturnBook;
using LibraryManagement.Application.Features.BorrowRecords.Queries.GetMyBorrowRecords;
using LibraryManagement.Application.Features.BorrowRecords.Queries.GetOverdueRecords;
using LibraryManagement.Application.Features.BorrowRecords.Queries.GetActiveRecords;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/borrow-records")]
[Authorize]
public class BorrowRecordsController : ControllerBase
{
    private readonly ISender _sender;

    public BorrowRecordsController(ISender sender)
    {
        _sender = sender;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(
            ClaimTypes.NameIdentifier)!);

    // POST /api/borrow-records
    [HttpPost]
    [Authorize(Roles = "READER")]
    public async Task<IActionResult> Borrow(
        [FromBody] BorrowBookRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(
                new BorrowBookCommand(
                    CurrentUserId,
                    request.BookId),
                ct);

            return Created(
                $"/api/borrow-records/{result.Id}",
                result);
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
                message = "Book stock changed. Please retry."
            });
        }
    }

    // PUT /api/borrow-records/{id}/return
    [HttpPut("{id:guid}/return")]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> Return(
        Guid id,
        CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(
                new ReturnBookCommand(
                    id,
                    CurrentUserId),
                ct);

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
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "Book stock changed. Please retry."
            });
        }
    }

    // GET /api/borrow-records/active
    [HttpGet("active")]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> GetActive(CancellationToken ct)
        => Ok(await _sender.Send(new GetActiveRecordsQuery(), ct));

    // GET /api/borrow-records/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRecords(
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new GetMyBorrowRecordsQuery(CurrentUserId),
            ct);

        return Ok(result);
    }

    // GET /api/borrow-records/overdue
    [HttpGet("overdue")]
    [Authorize(Roles = "ADMIN,LIBRARIAN")]
    public async Task<IActionResult> GetOverdue(
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new GetOverdueRecordsQuery(),
            ct);

        return Ok(result);
    }
}

public record BorrowBookRequest(Guid BookId);
