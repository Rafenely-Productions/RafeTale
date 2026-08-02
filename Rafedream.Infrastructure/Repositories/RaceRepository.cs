using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Rafedream.Infrastructure.Repositories;

public class RaceRepository : Repository<Race>, IRaceRepository
{
    public RaceRepository(RafedreamDbContext context) : base(context) { }

    public async Task<List<Race>> GetRacesWithTraitsAndSubraces(Expression<Func<Race, bool>>? filter, params Expression<Func<Race, object>>[] includes)
    {
        return await _context.Races
            .Include(c => c.SubRaces)
            .Include(c => c.Traits)
                .ThenInclude(p => p.Modifiers)
            .ToListAsync();
    }
}