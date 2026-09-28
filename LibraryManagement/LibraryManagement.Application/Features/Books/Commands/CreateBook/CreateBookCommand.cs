using System;
using LibraryManagement.Application.Features.Books.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    string ISBN,
    Guid CategoryId,
    Guid AuthorId,
    int TotalCopies,
    decimal Price
) : IRequest<BookDto>;
