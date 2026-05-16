using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DnDreams.Infrastructure.Repositories
{
    internal class ItemTemplateRepository : IItemTemplateRepository
    {
        private readonly DnDreamsDbContext _context;
        public ItemTemplateRepository(DnDreamsDbContext context) => _context = context;

        public async Task AddAsync(ItemTemplate item)
        {
            await _context.Set<ItemTemplate>().AddAsync(item);
        }

        public async Task AddRangeAsync(IEnumerable<ItemTemplate> templates)
        {
            await _context.Set<ItemTemplate>().AddRangeAsync(templates);
        }

        public async Task<IEnumerable<ItemTemplate>> GetAllAsync()
        {
            return await _context.Set<ItemTemplate>().ToListAsync();
        }

        public async Task<ItemTemplate> GetByNameAsync(string name)
        {
            return await _context.ItemTemplates.FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}
