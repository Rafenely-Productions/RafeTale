using Rafedream.Application.Interfaces;
using Rafedream.Domain.Interfaces;
using Rafedream.Infrastructure.Extractors;
using Rafedream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Rafedream.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbPath)
    {
        services.AddDbContext<RafedreamDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath};Cache=Shared"));


        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataExtractor, ExcelDataExtractor>();
        return services;
    }
}