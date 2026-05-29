using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories;

public interface IClassDefinitionRepository : IRepository<ClassDefinition>
{
    Task<ClassDefinition?> GetByNameAsync(string name);
    Task<ClassDefinition> GetByIdAsync(Guid name);
    Task<List<ClassDefinition>> GetClassesWithFeatures();
}