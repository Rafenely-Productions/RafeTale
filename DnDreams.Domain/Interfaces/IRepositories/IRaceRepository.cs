using DnDreams.Domain.Entities;
using DnDreams.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories;

public interface IRaceRepository : IRepository<Race>
{
    Task<Race?> GetByNameAsync(string name);
    Task<Race?> GetByIdAsync(Guid id);
}