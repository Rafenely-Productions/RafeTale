using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Repositories
{
    public class ClassLevelProgressionRepository : Repository<ClassLevelProgression>, IClassLevelProgressionRepository
    {
        public ClassLevelProgressionRepository(DnDreamsDbContext context):base(context) { }

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
