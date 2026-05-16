using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces
{
    public interface IItemTemplateRepository
    {
        Task AddAsync(ItemTemplate item);
        Task AddRangeAsync(IEnumerable<ItemTemplate> templates);
        Task<IEnumerable<ItemTemplate>> GetAllAsync();
        Task<ItemTemplate> GetByNameAsync(string name);
    }
}
