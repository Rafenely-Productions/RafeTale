using RafeTale.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Domain.Interfaces.IRepositories;

public interface IFeatRepository : IRepository<Feat>
{
    Task<Feat> GetByNameAsync(string name);
}