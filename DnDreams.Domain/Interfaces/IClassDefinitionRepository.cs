using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IClassDefinitionRepository
{
    Task AddRangeAsync(IEnumerable<ClassDefinition> classes);
    Task<ClassDefinition?> GetByNameAsync(string name);
    Task AddAsync(ClassDefinition cls);
    Task<List<ClassDefinition>> GetAllAsync();
}