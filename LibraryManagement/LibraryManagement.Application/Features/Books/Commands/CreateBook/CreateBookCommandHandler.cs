using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Books.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler
    : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IApplicationDbContext _context;

    public CreateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookDto> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.ISBN))
            throw new ArgumentException(
                "Title and ISBN are required.");

        if (request.TotalCopies < 0 || request.Price < 0)
            throw new ArgumentException(
                "Copies and price cannot be negative.");

        var isbn = request.ISBN.Trim();

        var exists = await _context.Books
            .IgnoreQueryFilters()
            .AnyAsync(x => x.ISBN == isbn, cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "ISBN already exists.");

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId,
                cancellationToken);

        if (category == null)
            throw new KeyNotFoundException(
                "Category not found.");

        var author = await _context.Authors
            .FirstOrDefaultAsync(
                x => x.Id == request.AuthorId,
                cancellationToken);

        if (author == null)
            throw new KeyNotFoundException(
                "Author not found.");

        var book = new Book
        {
            Title = request.Title.Trim(),
            ISBN = isbn,
            CategoryId = category.Id,
            AuthorId = author.Id,
            TotalCopies = request.TotalCopies,
            AvailableCopies = request.TotalCopies,
            Price = request.Price,
            Version = 1
        };

        _context.Books.Add(book);

        await _context.SaveChangesAsync(cancellationToken);

        return new BookDto(
            book.Id,
            book.Title,
            book.ISBN,
            book.CategoryId,
            category.Name,
            book.AuthorId,
            author.Name,
            book.TotalCopies,
            book.AvailableCopies,
            book.Price
        );
    }
}
