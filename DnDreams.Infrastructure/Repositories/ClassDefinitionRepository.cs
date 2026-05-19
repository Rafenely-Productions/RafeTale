using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using DnDreams.Infrastructure.Persistence;

namespace DnDreams.Infrastructure.Repositories;

public class ClassDefinitionRepository : IClassDefinitionRepository
{
    private readonly DnDreamsDbContext _context;
    public ClassDefinitionRepository(DnDreamsDbContext context) => _context = context;

    public async Task AddRangeAsync(IEnumerable<ClassDefinition> classes)
    {
        await _context.ClassDefinitions.AddRangeAsync(classes);
    }
    public async Task<ClassDefinition?> GetByNameAsync(string name)
    {
        return await _context.ClassDefinitions.FirstOrDefaultAsync(c => c.Name == name);
    }
    public async Task AddAsync(ClassDefinition cls)
    {
        await _context.ClassDefinitions.AddAsync(cls);
    }
    public async Task<List<ClassDefinition>> GetAllAsync()
    {
        return await _context.ClassDefinitions.Include(c => c.Progressions).ThenInclude(p => p.Features).ToListAsync();
    }

    public async Task<ClassDefinition> GetByIdAsync(Guid name)
    {
        return await _context.ClassDefinitions.FirstOrDefaultAsync(c => c.Id == name);
    }
}