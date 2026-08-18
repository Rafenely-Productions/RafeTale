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
    internal class ItemTemplateRepository(RafeTaleDbContext context) : Repository<ItemTemplate>(context), IItemTemplateRepository
    {
        public async Task<ItemTemplate> GetByNameAsync(string name)
        {
            return await _context.ItemTemplates.FirstOrDefaultAsync(t => t.TechnicalName == name)
            ?? throw new KeyNotFoundException($"ItemTemplate with name '{name}' not found.");
        }
    }
}
