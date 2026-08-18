using RafeTale.Application.Interfaces;
using RafeTale.Domain.Interfaces;
using RafeTale.Infrastructure.Extraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RafeTale.Infrastructure.Persistence;

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