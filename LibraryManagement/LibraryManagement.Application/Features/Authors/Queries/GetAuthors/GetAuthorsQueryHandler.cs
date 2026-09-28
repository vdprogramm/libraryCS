using LibraryManagement.Application.Features.Authors.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Authors.Queries.GetAuthors;

public class GetAuthorsQueryHandler
    : IRequestHandler<GetAuthorsQuery, List<AuthorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAuthorsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuthorDto>> Handle(
        GetAuthorsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Authors
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new AuthorDto(
                x.Id,
                x.Name,
                x.Biography
            ))
            .ToListAsync(cancellationToken);
    }
}
