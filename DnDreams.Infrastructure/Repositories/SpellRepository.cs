using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence;

public class SpellRepository : ISpellRepository
{
    private readonly DnDreamsDbContext _context;
    public SpellRepository(DnDreamsDbContext context) => _context = context;

    public async Task AddRangeAsync(IEnumerable<Spell> spells)
    {
        foreach (var spell in spells)
        {
            if (!await _context.Set<Spell>().AnyAsync(s => s.Name == spell.Name))
            {
                await _context.Set<Spell>().AddAsync(spell);
            }
        }
    }

    public async Task<IEnumerable<Spell>> GetAllAsync()
    {
        return await _context.Set<Spell>().ToListAsync();
    }
}