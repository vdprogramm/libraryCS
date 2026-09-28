using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();
    public DbSet<Penalty> Penalties => Set<Penalty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.Username)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(255).IsRequired();

            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(150);
        });

        // Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name)
                .HasMaxLength(50).IsRequired();
        });

        // UserRole: composite primary key
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.RoleId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId);
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(150).IsRequired();
        });

        // Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(150).IsRequired();
        });

        // Book
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(x => x.ISBN).IsUnique();

            entity.HasQueryFilter(x => !x.IsDeleted);

            entity.Property(x => x.Title)
                .HasMaxLength(255).IsRequired();

            entity.Property(x => x.ISBN)
                .HasMaxLength(20).IsRequired();

            entity.Property(x => x.Price)
                .HasPrecision(18, 2);

            entity.Property(x => x.Version)
                .IsConcurrencyToken();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Books_TotalCopies",
                    "\"TotalCopies\" >= 0");

                t.HasCheckConstraint(
                    "CK_Books_AvailableCopies",
                    "\"AvailableCopies\" >= 0 AND " +
                    "\"AvailableCopies\" <= \"TotalCopies\"");

                t.HasCheckConstraint(
                    "CK_Books_Price",
                    "\"Price\" >= 0");
            });

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Books)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Author)
                .WithMany(x => x.Books)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BorrowRecord
        modelBuilder.Entity<BorrowRecord>(entity =>
        {
            entity.Property(x => x.Status)
                .HasMaxLength(30).IsRequired();

            entity.HasOne(x => x.Reader)
                .WithMany(x => x.BorrowRecords)
                .HasForeignKey(x => x.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ReturnedBy)
                .WithMany()
                .HasForeignKey(x => x.ReturnedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Book)
                .WithMany(x => x.BorrowRecords)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
            {
                x.ReaderId,
                x.Status
            });
        });

        // Penalty
        modelBuilder.Entity<Penalty>(entity =>
        {
            entity.Property(x => x.FineAmount)
                .HasPrecision(18, 2);

            entity.Property(x => x.PaymentStatus)
                .HasMaxLength(30).IsRequired();

            entity.HasOne(x => x.BorrowRecord)
                .WithOne(x => x.Penalty)
                .HasForeignKey<Penalty>(x => x.BorrowRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Penalties_FineAmount",
                    "\"FineAmount\" >= 0"));
        });
    }
}
