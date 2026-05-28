using DnDreams.Domain.Entities;

namespace DnDreams.Domain.Interfaces.IRepositories
{
    public interface IClassLevelProgressionRepository : IRepository<ClassLevelProgression>
    {
        Task AddProgressionAsync(ClassLevelProgression prog);
        Task AddProgressionsRangeAsync(List<ClassLevelProgression> progressions);
        Task<ClassLevelProgression?> GetProgressionsByClassAndLevelAsync(Guid classId, int level);
        Task<ClassLevelProgression> GetByNameAsync(string name);
    }
}
