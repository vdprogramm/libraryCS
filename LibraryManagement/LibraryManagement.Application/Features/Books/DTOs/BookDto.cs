using System;

namespace LibraryManagement.Application.Features.Books.DTOs;

public record BookDto(
    Guid Id,
    string Title,
    string ISBN,
    Guid CategoryId,
    string CategoryName,
    Guid AuthorId,
    string AuthorName,
    int TotalCopies,
    int AvailableCopies,
    decimal Price
);
