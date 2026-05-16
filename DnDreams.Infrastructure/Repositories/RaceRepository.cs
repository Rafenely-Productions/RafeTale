using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DnDreams.Infrastructure.Persistence;

namespace DnDreams.Infrastructure.Repositories;

public class RaceRepository : IRaceRepository
{
    private readonly DnDreamsDbContext _context;
    public RaceRepository(DnDreamsDbContext context) => _context = context;

    public async Task AddAsync(Race race)
    {
        await _context.Races.AddAsync(race);
    }

    public async Task AddRangeAsync(IEnumerable<Race> races)
    {
        await _context.Races.AddRangeAsync(races);
    }

    public async Task<List<Race>> GetAllAsync()
    {
        return await _context.Races.ToListAsync();
    }

    public async Task<Race?> GetByNameAsync(string name)
    {
        return await _context.Races.FirstOrDefaultAsync(r => r.Name == name);
    }
}