using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Repositories;

public class SpellRepository(RafeTaleDbContext context) : Repository<Spell>(context), ISpellRepository
{
    public async Task<Spell> GetByNameAsync(string name)
    {
        return await _context.Set<Spell>().FirstOrDefaultAsync(s => s.TechnicalName == name)
        ?? throw new KeyNotFoundException($"Spell with name '{name}' not found.");
    }
}