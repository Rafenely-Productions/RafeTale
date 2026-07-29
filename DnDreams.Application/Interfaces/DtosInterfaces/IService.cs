using DnDreams.Domain.Helpers;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System.Linq.Expressions;

namespace DnDreams.Application.Interfaces.DtosInterfaces
{
    public interface IService<TDto,TEntity> where TDto : class where TEntity : class
    {
        Task<TDto> ArmDto(TEntity entity);
        TDto ArmDto(TEntity entity, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords = null);
        Task<List<TDto>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, Action<IncludeAggregator<TEntity>>? includes = null);
        Task<TDto> GetByIdAsync(Guid id, Action<IncludeAggregator<TEntity>>? includes = null);
    }
}
