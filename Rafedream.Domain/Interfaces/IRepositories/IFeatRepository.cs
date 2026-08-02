using Rafedream.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Domain.Interfaces.IRepositories;

public interface IFeatRepository : IRepository<Feat>
{
    Task<Feat> GetByNameAsync(string name);
}