using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Category> Categories { get; }
    DbSet<Author> Authors { get; }
    DbSet<Book> Books { get; }
    DbSet<BorrowRecord> BorrowRecords { get; }
    DbSet<Penalty> Penalties { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
