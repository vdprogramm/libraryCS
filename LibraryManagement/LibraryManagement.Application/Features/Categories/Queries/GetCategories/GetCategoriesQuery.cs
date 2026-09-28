using System.Collections.Generic;
using MediatR;
using LibraryManagement.Application.Features.Categories.DTOs;

namespace LibraryManagement.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery()
    : IRequest<List<CategoryDto>>;
