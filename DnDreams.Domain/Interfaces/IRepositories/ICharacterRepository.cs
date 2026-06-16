using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories;

public interface ICharacterRepository : IRepository<Character>
{
    Task<IEnumerable<Character>> GetAllWithDetailsAsync();
    Task<Character> GetByNameAsync(string name);
    Task<Character> GetByIdAsync(Guid id);
    Task RemoveAsync(Character existingChar);
}