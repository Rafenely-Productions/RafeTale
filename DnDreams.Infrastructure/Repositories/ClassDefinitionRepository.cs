using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using DnDreams.Infrastructure.Persistence;
using DnDreams.Domain.Interfaces.IRepositories;
using System.Linq.Expressions;
using DnDreams.Domain.Modifiers;

namespace DnDreams.Infrastructure.Repositories;

public class ClassDefinitionRepository : Repository<ClassDefinition>, IClassDefinitionRepository
{
    public ClassDefinitionRepository(DnDreamsDbContext context) : base(context) { }

    public async Task<ClassDefinition?> GetByNameAsync(string name)
    {
        return await _context.ClassDefinitions.FirstOrDefaultAsync(c => c.TechnicalName == name);
    }
    public async Task<List<ClassDefinition>> GetClassesWithFeatures(Expression<Func<ClassDefinition, bool>>? filter,params Expression<Func<ClassDefinition, object>>[] includes)
    {
        return await _context.ClassDefinitions
            .Include(c => c.Progressions)
                .ThenInclude(p => p.Features)
            .ToListAsync();
    }
}