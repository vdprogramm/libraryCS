using LibraryManagement.Application.Interfaces;
using LibraryManagement.Infrastructure.Authentication;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string is missing.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<
            ITransactionManager,
            LibraryManagement.Infrastructure.Persistence.Transactions.EfTransactionManager>();

        services.AddScoped<
            IRowLockService,
            LibraryManagement.Infrastructure.Persistence.Transactions.RowLockService>();

        return services;
    }
}
