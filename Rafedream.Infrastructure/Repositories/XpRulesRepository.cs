using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Persistence;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.EntityFrameworkCore;

namespace Rafedream.Infrastructure.Repositories
{
    public class XpRulesRepository : Repository<XpRules>, IXpRulesRepository
    {
        public XpRulesRepository(RafedreamDbContext context) : base(context){}

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
