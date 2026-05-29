using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DnDreams.Infrastructure.Repositories;

public class RaceRepository : Repository<Race>, IRaceRepository
{
    public RaceRepository(DnDreamsDbContext context) : base(context) { }

    public async Task<Race?> GetByIdAsync(Guid id, params Expression<Func<Race, object>>[] includes)
    {
        return await _context.Races.Include(r => r.Traits).FirstOrDefaultAsync(x => x.Id == id);
    }
}