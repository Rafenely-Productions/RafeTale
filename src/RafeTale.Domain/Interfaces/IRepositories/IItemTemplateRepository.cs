using RafeTale.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Domain.Interfaces.IRepositories
{
    public interface IItemTemplateRepository : IRepository<ItemTemplate>
    {
        Task<ItemTemplate> GetByNameAsync(string name);
    }
}
