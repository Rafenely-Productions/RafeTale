using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure.Persistence;
using DnDreams.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DnDreams.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<DnDreamsDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<ExcelImportService>();
        services.AddScoped<CharacterManager>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}