using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Books.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler
    : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IApplicationDbContext _context;

    public GetBookByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookDto?> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Books
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
