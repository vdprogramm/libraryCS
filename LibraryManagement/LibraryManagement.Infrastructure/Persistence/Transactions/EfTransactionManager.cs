using System;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagement.Infrastructure.Persistence.Transactions;

public class EfTransactionManager : ITransactionManager
{
    private readonly ApplicationDbContext _context;

    public EfTransactionManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IAppTransaction> BeginAsync(
        CancellationToken ct = default)
    {
        var transaction =
            await _context.Database.BeginTransactionAsync(ct);

        return new EfAppTransaction(transaction);
    }
}

public class EfAppTransaction : IAppTransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfAppTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Task CommitAsync(CancellationToken ct = default)
    {
        return _transaction.CommitAsync(ct);
    }

    public ValueTask DisposeAsync()
    {
        return _transaction.DisposeAsync();
    }
}
