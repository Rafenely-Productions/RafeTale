using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly DnDreamsDbContext _context;

        public Repository(DnDreamsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T? data)
        {
            await _context.Set<T>().AddAsync(data!);
        }

        public async Task AddRangeAsync(IEnumerable<T?> list)
        {
            await _context.Set<T>().AddRangeAsync(list!);
        }

        public async Task<IEnumerable<T?>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T?>> GetManyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T?>> GetAllAsync(Expression<Func<T, bool>>? filter, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = GetValues(filter, includes);

            return await query.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = GetValues(x => x.Id == id, includes);

            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<T> GetValues(Expression<Func<T, bool>>? filter, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query;
        }
    }
}
