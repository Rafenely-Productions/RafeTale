using Rafedream.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Domain.Interfaces.IRepositories
{
    public interface IXpRulesRepository : IRepository<XpRules>
    {
        Task<XpRules> GetByLevelAsync(int level);
        Task<Dictionary<int, int>> GetXpThresholdsAsync();
    }
}
