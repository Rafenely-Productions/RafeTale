using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IFeatRepository
{
    Task AddRangeAsync(IEnumerable<Feat> feats);
    Task<IEnumerable<Feat>> GetAllAsync();
}