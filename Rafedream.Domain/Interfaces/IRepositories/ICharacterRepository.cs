using Rafedream.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Domain.Interfaces.IRepositories;

public interface ICharacterRepository : IRepository<Character>
{
    Task<IEnumerable<Character>> GetAllWithDetailsAsync();
    Task<Character> GetByNameAsync(string name);
    Task<Character> GetByIdAsync(Guid id);
    Task RemoveAsync(Character existingChar);
}