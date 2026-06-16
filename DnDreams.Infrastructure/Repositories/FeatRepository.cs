using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Repositories;

public class FeatRepository : Repository<Feat>, IFeatRepository
{
    public FeatRepository(DnDreamsDbContext context) : base(context) { }

    public async Task<Feat> GetByNameAsync(string name)
    {
        return await _context.Set<Feat>().FirstOrDefaultAsync(f => f.TechnicalName == name);
    }
}