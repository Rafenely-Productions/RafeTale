using Rafedream.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Domain.Interfaces.IRepositories;

public interface ISpellRepository : IRepository<Spell>
{
    Task<Spell> GetByNameAsync(string name);
}