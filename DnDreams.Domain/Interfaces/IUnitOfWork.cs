using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using System;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Race> Races { get; }
    IClassDefinitionRepository ClassDefinitions { get; }
    IClassLevelProgressionRepository ClassLevelProgressions { get; }
    ICharacterRepository Characters { get; }
    IFeatureRepository Features { get; }
    IXpRulesRepository XpRules { get; }
    IFeatRepository Feats { get; }
    IRepository<Background> Backgrounds { get; }
    ISpellRepository Spells { get; }
    IItemTemplateRepository ItemTemplates { get; }
    ILocalizedContentRepository LocalizedContents { get; }
    IRepository<Trait> Traits { get; }
    IRepository<Language> Languages { get; }
    IRepository<SubRace> SubRaces { get; }
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}