using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Infrastructure.Repositories;

public class FeatRepository : Repository<Feat>, IFeatRepository
{
    public FeatRepository(RafedreamDbContext context) : base(context) { }

    public async Task<Feat> GetByNameAsync(string name)
    {
        return await _context.Set<Feat>().FirstOrDefaultAsync(f => f.TechnicalName == name);
    }
}