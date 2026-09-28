using LibraryManagement.Application.Features.Categories.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(
                x.Id,
                x.Name,
                x.Description
            ))
            .ToListAsync(cancellationToken);
    }
}
