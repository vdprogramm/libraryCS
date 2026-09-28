using System;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Books.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommandHandler
    : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITransactionManager _transactions;
    private readonly IRowLockService _locks;

    public UpdateBookCommandHandler(
        IApplicationDbContext context,
        ITransactionManager transactions,
        IRowLockService locks)
    {
        _context = context;
        _transactions = transactions;
        _locks = locks;
    }

    public async Task<BookDto> Handle(
        UpdateBookCommand request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.ISBN))
        {
            throw new ArgumentException(
                "Title and ISBN are required.");
        }

        if (request.TotalCopies < 0 || request.Price < 0)
        {
            throw new ArgumentException(
                "Invalid copies or price.");
        }

        await using var transaction =
            await _transactions.BeginAsync(ct);

        // Lock book before reading its stock.
        var book = await _locks.LockBookAsync(
            request.Id, ct);

        if (book == null || book.IsDeleted)
            throw new KeyNotFoundException(
                "Book not found.");

        var isbn = request.ISBN.Trim();

        var duplicate = await _context.Books
            .IgnoreQueryFilters()
            .AnyAsync(
                x => x.ISBN == isbn && x.Id != request.Id,
                ct);

        if (duplicate)
            throw new InvalidOperationException(
                "ISBN already exists.");

        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId,
                ct);

        if (category == null)
            throw new KeyNotFoundException(
                "Category not found.");

        var author = await _context.Authors
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.AuthorId,
                ct);

        if (author == null)
            throw new KeyNotFoundException(
                "Author not found.");

        // The locked row contains the latest committed stock.
        var borrowedCopies =
            book.TotalCopies - book.AvailableCopies;

        if (borrowedCopies < 0)
            throw new InvalidOperationException(
                "Invalid stock data.");

        if (request.TotalCopies < borrowedCopies)
            throw new InvalidOperationException(
                "Total copies cannot be less than borrowed copies.");

        book.Title = request.Title.Trim();
        book.ISBN = isbn;
        book.CategoryId = category.Id;
        book.AuthorId = author.Id;
        book.TotalCopies = request.TotalCopies;
        book.AvailableCopies =
            request.TotalCopies - borrowedCopies;
        book.Price = request.Price;

        book.Version++;

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

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
