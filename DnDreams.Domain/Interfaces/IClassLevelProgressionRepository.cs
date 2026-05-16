using DnDreams.Domain.Entities;

namespace DnDreams.Domain.Interfaces
{
    public interface IClassLevelProgressionRepository
    {
        Task AddProgressionAsync(ClassLevelProgression prog);
        Task AddProgressionsRangeAsync(List<ClassLevelProgression> progressions);
        Task<ClassLevelProgression?> GetProgressionsByClassAndLevelAsync(Guid classId, int level);
        Task<List<ClassLevelProgression>> GetAllAsync();
        Task<ClassLevelProgression> GetByNameAsync(string name);
    }
}
