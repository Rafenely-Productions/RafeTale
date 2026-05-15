using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using DnDreams.Infrastructure.Persistence;

namespace DnDreams.Infrastructure.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly DnDreamsDbContext _context;
    public ClassRepository(DnDreamsDbContext context) => _context = context;

    public async Task AddRangeAsync(IEnumerable<ClassDefinition> classes)
    {
        foreach (var cls in classes)
        {
            if (!await _context.ClassDefinitions.AnyAsync(c => c.Name == cls.Name))
            {
                await _context.ClassDefinitions.AddAsync(cls);
            }
        }
    }

    public async Task AddProgressionsRangeAsync(IEnumerable<ClassLevelProgression> progressions)
    {
        await _context.ClassLevelProgressions.AddRangeAsync(progressions);
    }

    public async Task<ClassDefinition?> GetByNameAsync(string name)
    {
        return await _context.ClassDefinitions.FirstOrDefaultAsync(c => c.Name == name);
    }
    public async Task<ClassLevelProgression?> GetProgressionsByClassAndLevelAsync(Guid classId, int level)
    {
        return await _context.ClassLevelProgressions
            .Include(p => p.Features)
            .FirstOrDefaultAsync(p => p.ClassDefId == classId && p.Level == level);
    }
}