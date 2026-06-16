using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories;

public interface ISpellRepository : IRepository<Spell>
{
    Task<Spell> GetByNameAsync(string name);
}