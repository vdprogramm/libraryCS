using System;
using LibraryManagement.Application.Features.Books.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook;

public record UpdateBookCommand(
    Guid Id,
    string Title,
    string ISBN,
    Guid CategoryId,
    Guid AuthorId,
    int TotalCopies,
    decimal Price
) : IRequest<BookDto>;
