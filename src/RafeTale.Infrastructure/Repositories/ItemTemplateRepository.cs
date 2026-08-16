using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace RafeTale.Infrastructure.Repositories
{
    internal class ItemTemplateRepository : Repository<ItemTemplate>, IItemTemplateRepository
    {
        public ItemTemplateRepository(RafeTaleDbContext context) : base(context) { }
        public async Task<ItemTemplate> GetByNameAsync(string name)
        {
            return await _context.ItemTemplates.FirstOrDefaultAsync(t => t.TechnicalName == name);
        }
    }
}
