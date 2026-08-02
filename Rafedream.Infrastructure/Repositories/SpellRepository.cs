using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Infrastructure.Persistence;

public class SpellRepository : Repository<Spell>, ISpellRepository
{
    public SpellRepository(RafedreamDbContext context) : base(context) { }

    public async Task<Spell> GetByNameAsync(string name)
    {
        return await _context.Set<Spell>().FirstOrDefaultAsync(s => s.TechnicalName == name);
    }
}