using RafeTale.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Domain.Interfaces.IRepositories;

public interface ISpellRepository : IRepository<Spell>
{
    Task<Spell> GetByNameAsync(string name);
}