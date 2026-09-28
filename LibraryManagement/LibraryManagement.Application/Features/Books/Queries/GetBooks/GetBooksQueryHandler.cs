using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Books.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Books.Queries.GetBooks;

public class GetBooksQueryHandler
    : IRequestHandler<GetBooksQuery, PagedResult<BookDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBooksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BookDto>> Handle(
        GetBooksQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim().ToLower();

            query = query.Where(x =>
                x.Title.ToLower().Contains(keyword) ||
                x.ISBN.ToLower().Contains(keyword) ||
                x.Author.Name.ToLower().Contains(keyword));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(
                x => x.CategoryId == request.CategoryId.Value);
        }

        if (request.AuthorId.HasValue)
        {
            query = query.Where(
                x => x.AuthorId == request.AuthorId.Value);
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        var books = await query
            .OrderBy(x => x.Title)
            .ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BookDto(
                x.Id,
                x.Title,
                x.ISBN,
                x.CategoryId,
                x.Category.Name,
                x.AuthorId,
                x.Author.Name,
                x.TotalCopies,
                x.AvailableCopies,
                x.Price
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<BookDto>(
            books,
            pageNumber,
            pageSize,
            totalCount,
            (int)Math.Ceiling((double)totalCount / pageSize)
        );
    }
}
