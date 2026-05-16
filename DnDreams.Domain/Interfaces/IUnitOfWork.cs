using System;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRaceRepository Races { get; }
    IClassDefinitionRepository ClassDefinitions { get; }
    IClassLevelProgressionRepository ClassLevelProgressions { get; }
    ICharacterRepository Characters { get; }
    IFeatureRepository Features { get; }
    IXpRulesRepository XpRules { get; }
    IFeatRepository Feats { get; }
    ISpellRepository Spells { get; }
    IItemTemplateRepository ItemTemplates { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}