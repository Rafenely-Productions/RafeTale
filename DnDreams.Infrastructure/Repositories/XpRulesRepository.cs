using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure.Persistence;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.EntityFrameworkCore;

namespace DnDreams.Infrastructure.Repositories
{
    internal class XpRulesRepository : IXpRulesRepository
    {
        private readonly DnDreamsDbContext _context;
        public XpRulesRepository(DnDreamsDbContext context) => _context = context;

        public async Task AddRangeAsync(IEnumerable<XpRules> xpRules)
        {
            await _context.XpRules.AddRangeAsync(xpRules);
        }

        public async Task<Dictionary<int, int>> GetXpThresholdsAsync()
        {
            return await _context.XpRules.ToDictionaryAsync(x => x.Level, x => x.RequiredXp);
        }
    }
}
