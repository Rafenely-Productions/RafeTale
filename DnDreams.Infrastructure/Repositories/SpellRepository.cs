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

    public async Task AddAsync(Spell spell)
    {
        await _context.Set<Spell>().AddAsync(spell);
    }

    public async Task AddRangeAsync(IEnumerable<Spell> spells)
    {
        await _context.Set<Spell>().AddRangeAsync(spells);
    }

    public async Task<IEnumerable<Spell>> GetAllAsync()
    {
        return await _context.Set<Spell>().ToListAsync();
    }

    public async Task<Spell> GetByNameAsync(string name)
    {
        return await _context.Set<Spell>().FirstOrDefaultAsync(s => s.Name == name);
    }
}