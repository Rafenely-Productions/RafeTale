using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence;

public class SpellRepository : Repository<Spell>, ISpellRepository
{
    public SpellRepository(DnDreamsDbContext context) : base(context) { }

    public async Task<Spell> GetByNameAsync(string name)
    {
        return await _context.Set<Spell>().FirstOrDefaultAsync(s => s.Name == name);
    }
}