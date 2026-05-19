using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IRaceRepository
{
    Task AddAsync(Race race);
    Task AddRangeAsync(IEnumerable<Race> races);
    Task<List<Race>> GetAllAsync();
    Task<Race?> GetByNameAsync(string name);
    Task<Race?> GetByIdAsync(Guid id);

}