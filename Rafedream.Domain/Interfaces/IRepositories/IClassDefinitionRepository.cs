using Rafedream.Domain.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rafedream.Domain.Interfaces.IRepositories;

public interface IClassDefinitionRepository : IRepository<ClassDefinition>
{
    Task<List<ClassDefinition>> GetClassesWithFeatures(Expression<Func<ClassDefinition, bool>>? filter, params Expression<Func<ClassDefinition, object>>[] includes);
}