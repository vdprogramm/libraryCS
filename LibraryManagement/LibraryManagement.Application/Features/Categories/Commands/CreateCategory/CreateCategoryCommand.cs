using MediatR;
using LibraryManagement.Application.Features.Categories.DTOs;

namespace LibraryManagement.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description
) : IRequest<CategoryDto>;
