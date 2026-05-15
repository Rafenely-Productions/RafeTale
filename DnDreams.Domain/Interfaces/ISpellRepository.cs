using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface ISpellRepository
{
    Task AddRangeAsync(IEnumerable<Spell> spells);
    Task<IEnumerable<Spell>> GetAllAsync();
}