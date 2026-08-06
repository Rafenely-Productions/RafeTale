using RafeTale.Application.Interfaces;
using RafeTale.Domain.Interfaces;
using RafeTale.Infrastructure.Extractors;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RafeTale.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<RafeTaleDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath};Cache=Shared"));


        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataExtractor, ExcelDataExtractor>();
        return services;
    }
}