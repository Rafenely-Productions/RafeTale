using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface ISpellRepository
{
    Task AddAsync(Spell spell);
    Task AddRangeAsync(IEnumerable<Spell> spells);
    Task<IEnumerable<Spell>> GetAllAsync();
    Task<Spell> GetByNameAsync(string name);
}