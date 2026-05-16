using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IFeatRepository
{
    Task AddAsync(Feat feat);
    Task AddRangeAsync(IEnumerable<Feat> feats);
    Task<IEnumerable<Feat>> GetAllAsync();
    Task<Feat> GetByNameAsync(string name);
}