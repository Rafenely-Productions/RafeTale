using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DnDreams.Infrastructure.Repositories
{
    internal class ItemTemplateRepository : Repository<ItemTemplate>, IItemTemplateRepository
    {
        public ItemTemplateRepository(DnDreamsDbContext context) : base(context) { }
        public async Task<ItemTemplate> GetByNameAsync(string name)
        {
            return await _context.ItemTemplates.FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}
