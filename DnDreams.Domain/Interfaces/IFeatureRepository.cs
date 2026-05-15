using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IFeatureRepository
{
    Task<IEnumerable<Feature>> GetAllAsync();
}