using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Rafedream.Infrastructure.Repositories
{
    internal class ItemTemplateRepository : Repository<ItemTemplate>, IItemTemplateRepository
    {
        public ItemTemplateRepository(RafedreamDbContext context) : base(context) { }
        public async Task<ItemTemplate> GetByNameAsync(string name)
        {
            return await _context.ItemTemplates.FirstOrDefaultAsync(t => t.TechnicalName == name);
        }
    }
}
