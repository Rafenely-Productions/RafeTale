using RafeTale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using RafeTale.Infrastructure.Persistence;
using RafeTale.Domain.Interfaces.IRepositories;
using System.Linq.Expressions;
using RafeTale.Domain.Modifiers;

namespace RafeTale.Infrastructure.Repositories;

public class ClassDefinitionRepository(RafeTaleDbContext context) : Repository<ClassDefinition>(context), IClassDefinitionRepository
{
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