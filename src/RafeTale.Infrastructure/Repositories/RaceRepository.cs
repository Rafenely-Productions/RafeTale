using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RafeTale.Infrastructure.Repositories;

public class RaceRepository(RafeTaleDbContext context) : Repository<Race>(context), IRaceRepository
{
    public async Task<List<Race>> GetRacesWithTraitsAndSubraces(Expression<Func<Race, bool>>? filter, params Expression<Func<Race, object>>[] includes)
    {
        return await _context.Races
            .Include(c => c.Subraces)
            .Include(c => c.Traits)
                .ThenInclude(p => p.Modifiers)
            .ToListAsync();
    }
}