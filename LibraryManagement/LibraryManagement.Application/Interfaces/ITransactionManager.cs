using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Interfaces;

public interface ITransactionManager
{
    Task<IAppTransaction> BeginAsync(
        CancellationToken ct = default);
}

public interface IAppTransaction : IAsyncDisposable
{
    Task CommitAsync(
        CancellationToken ct = default);
}
