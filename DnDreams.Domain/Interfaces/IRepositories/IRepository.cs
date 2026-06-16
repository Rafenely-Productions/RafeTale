using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T? data);
        Task AddRangeAsync(IEnumerable<T?> list);
        Task<IEnumerable<T?>> GetAllAsync();
        Task<IEnumerable<T?>> GetAllAsync(Expression<Func<T, bool>>? filter,params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T?>> GetManyAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    }
}
