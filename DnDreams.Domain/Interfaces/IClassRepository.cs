using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IClassRepository
{
    Task AddRangeAsync(IEnumerable<ClassDefinition> classes);
    Task AddProgressionsRangeAsync(IEnumerable<ClassLevelProgression> progressions);
    Task<ClassDefinition?> GetByNameAsync(string name);
    Task<ClassLevelProgression?> GetProgressionsByClassAndLevelAsync(Guid classId, int level);
}