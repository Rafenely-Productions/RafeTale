using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.EntityFrameworkCore;

namespace RafeTale.Infrastructure.Repositories
{
    public class XpRulesRepository : Repository<XpRules>, IXpRulesRepository
    {
        public XpRulesRepository(RafeTaleDbContext context) : base(context){}

        public async Task<XpRules> GetByLevelAsync(int level)
        {
            return await _context.XpRules.FirstOrDefaultAsync(r => r.Level == level);
        }

        public async Task<Dictionary<int, int>> GetXpThresholdsAsync()
        {
            return await _context.XpRules.ToDictionaryAsync(x => x.Level, x => x.RequiredXp);
        }
    }
}
