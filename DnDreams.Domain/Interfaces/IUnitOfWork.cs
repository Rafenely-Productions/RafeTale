using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
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
    IRepository<Background> Backgrounds { get; }
    ISpellRepository Spells { get; }
    IItemTemplateRepository ItemTemplates { get; }
    ILocalizedContentRepository LocalizedContents { get; }
    IRepository<Trait> Traits { get; }
    IRepository<Language> Languages { get; }
    IRepository<SubRace> SubRaces { get; }
    IRepository<SchoolOfMagic> SchoolsOfMagic { get; }
    IRepository<Subclass> Subclasses { get; }
    IRepository<SubclassLevelProgression> SubclassLevelProgressions { get; }
    IRepository<Skill> Skills { get; }
    Task BeginTransactionAsync();
    Task CommitAsync();
    void ModifyState(object m);
    Task RollbackAsync();
    Task<int> SaveChangesAsync();

    void DetachOrUnchangeEntities<T>() where T : class;

    void SetUnchangedState(object entity);
    void PureCleanTrackerForCharacter();

    Task<bool> SpellSlotExistsAsync(Guid id);
    void TrackNewSpellSlot(CharacterSpellSlots cps);
}