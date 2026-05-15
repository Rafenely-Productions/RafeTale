using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface ICharacterRepository
{
    Task AddRangeAsync(IEnumerable<Character> characters);
    Task<IEnumerable<Character>> GetAllWithDetailsAsync();
}