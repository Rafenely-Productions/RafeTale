using DnDreams.Domain.DTOs;
using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories;

public interface IRaceRepository : IRepository<Race>
{
    Task<Race?> GetByIdAsync(Guid id, params Expression<Func<Race, object>>[] includes);
}