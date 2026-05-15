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

    public async Task AddRangeAsync(IEnumerable<Feat> feats)
    {
        foreach (var feat in feats)
        {
            if (!await _context.Set<Feat>().AnyAsync(f => f.Name == feat.Name))
            {
                await _context.Set<Feat>().AddAsync(feat);
            }
        }
    }

    public async Task<IEnumerable<Feat>> GetAllAsync()
    {
        return await _context.Set<Feat>().ToListAsync();
    }
}