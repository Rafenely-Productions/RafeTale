using RafeTale.Domain.Entities;

namespace RafeTale.Domain.Interfaces.IRepositories
{
    public interface IClassLevelProgressionRepository : IRepository<ClassLevelProgression>
    {
        Task AddProgressionsRangeAsync(List<ClassLevelProgression> progressions);
        Task<ClassLevelProgression?> GetProgressionsByClassAndLevelAsync(Guid classId, int level);
    }
}
