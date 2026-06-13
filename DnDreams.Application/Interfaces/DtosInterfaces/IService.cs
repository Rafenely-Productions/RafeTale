using DnDreams.Domain.Enums;
using System.Linq.Expressions;

namespace DnDreams.Application.Interfaces.DtosInterfaces
{
    public interface IService<TDto,TEntity> where TDto : class where TEntity : class
    {
        Task<TDto> ArmDto(TEntity entity);
        TDto ArmDto(TEntity entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords);
        Task<List<TDto>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, params Expression<Func<TEntity, object>>[] includes);
        Task<TDto> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);
    }
}
