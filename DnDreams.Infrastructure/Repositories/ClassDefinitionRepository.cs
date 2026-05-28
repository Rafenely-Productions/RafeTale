using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using DnDreams.Infrastructure.Persistence;
using DnDreams.Domain.Interfaces.IRepositories;

namespace DnDreams.Infrastructure.Repositories;

public class ClassDefinitionRepository : Repository<ClassDefinition>, IClassDefinitionRepository
{
    public ClassDefinitionRepository(DnDreamsDbContext context) : base(context) { }

    public async Task<ClassDefinition?> GetByNameAsync(string name)
    {
        return await _context.ClassDefinitions.FirstOrDefaultAsync(c => c.Name == name);
    }
    public async Task<ClassDefinition> GetByIdAsync(Guid name)
    {
        return await _context.ClassDefinitions.FirstOrDefaultAsync(c => c.Id == name);
    }
}