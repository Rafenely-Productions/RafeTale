using Rafedream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using Rafedream.Infrastructure.Persistence;
using Rafedream.Domain.Interfaces.IRepositories;

namespace Rafedream.Infrastructure.Repositories;

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(RafedreamDbContext context) : base(context) { }

    public async Task<IEnumerable<Character>> GetAllWithDetailsAsync()
    {
        return await _context.Characters
            .Include(c => c.Race)
            .Include(c => c.ClassDef)
            .Include(c => c.AcquiredFeatures)
            .Include(c => c.AcquiredFeats)
            .Include(c => c.KnownSpells)
            .Include(c => c.CharacterModifiers)
            .ToListAsync();
    }

    public async Task<Character> GetByNameAsync(string name)
    {
        return await _context.Characters.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Character> GetByIdAsync(Guid id)
    {
        return await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task RemoveAsync(Character existingChar)
    {
        await _context.Characters.Where(c => c.Id == existingChar.Id).ExecuteDeleteAsync();
    }
}