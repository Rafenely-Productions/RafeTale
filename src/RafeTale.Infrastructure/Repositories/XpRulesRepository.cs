using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.EntityFrameworkCore;

namespace RafeTale.Infrastructure.Repositories
{
    public class XpRulesRepository(RafeTaleDbContext context) : Repository<XpRules>(context), IXpRulesRepository
    {
        public async Task<XpRules> GetByLevelAsync(int level)
        {
            return await _context.XpRules.FirstOrDefaultAsync(r => r.Level == level)
            ?? throw new KeyNotFoundException($"XpRules with level '{level}' not found.");
        }

        public async Task<Dictionary<int, int>> GetXpThresholdsAsync()
        {
            return await _context.XpRules.ToDictionaryAsync(x => x.Level, x => x.RequiredXp);
        }
    }
}
