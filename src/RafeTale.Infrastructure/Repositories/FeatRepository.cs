using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Repositories;

public class FeatRepository(RafeTaleDbContext context) : Repository<Feat>(context), IFeatRepository
{
    public async Task<Feat> GetByNameAsync(string name)
    {
        return await _context.Set<Feat>().FirstOrDefaultAsync(f => f.TechnicalName == name)
        ?? throw new KeyNotFoundException($"Feat with name '{name}' not found.");
    }
}