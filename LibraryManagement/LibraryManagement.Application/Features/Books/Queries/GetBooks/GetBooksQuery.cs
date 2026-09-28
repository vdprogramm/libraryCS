using System;
using LibraryManagement.Application.Features.Books.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetBooks;

public record GetBooksQuery(
    string? Keyword,
    Guid? CategoryId,
    Guid? AuthorId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedResult<BookDto>>;
