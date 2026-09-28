using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Interfaces;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.DeleteBook;

public class DeleteBookCommandHandler
    : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ITransactionManager _transactions;
    private readonly IRowLockService _locks;

    public DeleteBookCommandHandler(
        IApplicationDbContext context,
        ITransactionManager transactions,
        IRowLockService locks)
    {
        _context = context;
        _transactions = transactions;
        _locks = locks;
    }

    public async Task<bool> Handle(
        DeleteBookCommand request,
        CancellationToken ct)
    {
        await using var transaction =
            await _transactions.BeginAsync(ct);

        var book = await _locks.LockBookAsync(
            request.Id, ct);

        if (book == null || book.IsDeleted)
            return false;

        book.IsDeleted = true;
        book.Version++;

        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return true;
    }
}
