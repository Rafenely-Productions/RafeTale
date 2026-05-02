using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DnDreams.Infrastructure.Persistence;

namespace DnDreams.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<DnDreamsDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        return services;
    }
}