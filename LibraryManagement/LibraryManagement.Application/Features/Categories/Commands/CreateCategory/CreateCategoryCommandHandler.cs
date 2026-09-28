using LibraryManagement.Application.Features.Categories.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException(
                "Category name is required.");

        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim()
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryDto(
            category.Id,
            category.Name,
            category.Description
        );
    }
}
