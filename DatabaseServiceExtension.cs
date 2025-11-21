using MarketingAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketingAPI;

public static class DatabaseServiceExtension
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        string? provider = configuration["DatabaseProvider"];

        switch (provider)
        {
#if DEBUG
            case "SQLite":
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
                break;
#endif
            case "MySQL":
                services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                break;

            default:
                throw new InvalidOperationException($"Unsupported database provider: {provider}");
        }

        return services;
    }
}