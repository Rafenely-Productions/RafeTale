using Rafedream.Domain.Helpers;
using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Query;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Infrastructure.Repositories
{
    public class Repository<T>(RafedreamDbContext context) : IRepository<T> where T : class, IEntity
    {
        protected readonly RafedreamDbContext _context = context;

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

        public async Task<IEnumerable<T?>> GetAllAsync(Expression<Func<T, bool>>? filter, Action<IncludeAggregator<T>>? includes = null)
        {
            IQueryable<T> query = GetValues(filter, includes);


            return await query.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(Guid id, Action<IncludeAggregator<T>>? includes = null)
        {
            IQueryable<T> query = GetValues(x => x.Id == id, includes);

            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<T> GetValues(Expression<Func<T, bool>>? filter, Action<IncludeAggregator<T>>? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                var aggregator = new IncludeAggregator<T>();
                includes(aggregator); // Esto llena la lista de strings 'IncludePaths'

                foreach (var path in aggregator.IncludePaths)
                {
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        query = query.Include(path); // EF Core devora los strings felices
                    }
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
