using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence;

public class SpellRepository : Repository<Spell>, ISpellRepository
{
    public SpellRepository(RafeTaleDbContext context) : base(context) { }

    public async Task<Spell> GetByNameAsync(string name)
    {
        return await _context.Set<Spell>().FirstOrDefaultAsync(s => s.TechnicalName == name);
    }
}