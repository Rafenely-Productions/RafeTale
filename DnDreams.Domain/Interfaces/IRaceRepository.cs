using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IRaceRepository
{
    Task AddRangeAsync(IEnumerable<Race> races);
    Task<Race?> GetByNameAsync(string name);
}