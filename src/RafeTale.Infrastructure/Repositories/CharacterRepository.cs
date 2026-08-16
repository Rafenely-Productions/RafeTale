using RafeTale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using RafeTale.Infrastructure.Persistence;
using RafeTale.Domain.Interfaces.IRepositories;

namespace RafeTale.Infrastructure.Repositories;

public class CharacterRepository(RafeTaleDbContext context) : Repository<Character>(context), ICharacterRepository
{
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