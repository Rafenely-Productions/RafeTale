using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure.Persistence;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.EntityFrameworkCore;

namespace DnDreams.Infrastructure.Repositories
{
    public class XpRulesRepository : IXpRulesRepository
    {
        private readonly DnDreamsDbContext _context;
        public XpRulesRepository(DnDreamsDbContext context) => _context = context;

        public async Task AddAsync(XpRules xp)
        {
            await _context.XpRules.AddAsync(xp);
        }

        public async Task AddRangeAsync(IEnumerable<XpRules> xpRules)
        {
            await _context.XpRules.AddRangeAsync(xpRules);

        }

        public async Task<List<XpRules>> GetAllAsync()
        {
            return await _context.XpRules.ToListAsync();
        }

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
