using Rafedream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using Rafedream.Infrastructure.Persistence;
using Rafedream.Domain.Interfaces.IRepositories;
using System.Linq.Expressions;
using Rafedream.Domain.Modifiers;

namespace Rafedream.Infrastructure.Repositories;

public class ClassDefinitionRepository : Repository<ClassDefinition>, IClassDefinitionRepository
{
    public ClassDefinitionRepository(RafedreamDbContext context) : base(context) { }

    public async Task<List<ClassDefinition>> GetClassesWithFeatures(Expression<Func<ClassDefinition, bool>>? filter,params Expression<Func<ClassDefinition, object>>[] includes)
    {
        return await _context.ClassDefinitions
            .Include(c=> c.Subclasses)
                .ThenInclude(sc=> sc.Progressions)
                    .ThenInclude(p=> p.Features)
            .Include(c=> c.SkillProficiencies)
            .Include(c => c.Progressions)
                .ThenInclude(p => p.Features)
            .ToListAsync();
    }
}