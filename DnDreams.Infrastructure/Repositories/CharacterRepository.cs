using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using DnDreams.Infrastructure.Persistence;

namespace DnDreams.Infrastructure.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly DnDreamsDbContext _context;
    public CharacterRepository(DnDreamsDbContext context) => _context = context;

    public async Task AddRangeAsync(IEnumerable<Character> characters)
    {
        await _context.Characters.AddRangeAsync(characters);
    }

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