using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Repositories;

public class FeatRepository : IFeatRepository
{
    private readonly DnDreamsDbContext _context;
    public FeatRepository(DnDreamsDbContext context) => _context = context;

    public async Task AddAsync(Feat feat)
    {
        await _context.Set<Feat>().AddAsync(feat);
    }

    public async Task AddRangeAsync(IEnumerable<Feat> feats)
    {
        await _context.Set<Feat>().AddRangeAsync(feats);
    }

    public async Task<IEnumerable<Feat>> GetAllAsync()
    {
        return await _context.Set<Feat>().ToListAsync();
    }

    public async Task<Feat> GetByNameAsync(string name)
    {
        return await _context.Set<Feat>().FirstOrDefaultAsync(f => f.Name == name);
    }
}