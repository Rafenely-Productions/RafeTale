using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces
{
    public interface IXpRulesRepository
    {
        Task AddAsync(XpRules xp);
        Task AddRangeAsync(IEnumerable<XpRules> xpRules);
        Task<List<XpRules>> GetAllAsync();
        Task<XpRules> GetByLevelAsync(int level);
        Task<System.Collections.Generic.Dictionary<int, int>> GetXpThresholdsAsync();
    }
}
