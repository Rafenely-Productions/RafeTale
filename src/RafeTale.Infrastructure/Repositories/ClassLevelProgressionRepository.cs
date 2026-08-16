using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Repositories
{
    public class ClassLevelProgressionRepository : Repository<ClassLevelProgression>, IClassLevelProgressionRepository
    {
        public ClassLevelProgressionRepository(RafeTaleDbContext context):base(context) { }

        public async Task AddProgressionsRangeAsync(List<ClassLevelProgression> progressions)
        {
            await _context.ClassLevelProgressions.AddRangeAsync(progressions);
        }
        public async Task<ClassLevelProgression?> GetProgressionsByClassAndLevelAsync(Guid classId, int level)
        {
            return await _context.ClassLevelProgressions
                .Include(p => p.Features)
                .FirstOrDefaultAsync(p => p.ClassDefId == classId && p.Level == level);
        }
    }
}
