using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface ICharacterRepository
{
    Task AddRangeAsync(IEnumerable<Character> characters);
    Task<IEnumerable<Character>> GetAllWithDetailsAsync();
    Task<Character> GetByNameAsync(string name);
    Task<Character> GetByIdAsync(Guid id);
    Task RemoveAsync(Character existingChar);
}