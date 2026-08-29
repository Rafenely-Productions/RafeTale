using Microsoft.EntityFrameworkCore;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Entities.Rules;
using RafeTale.Domain.Modifiers;
using System.Text.Json;

namespace RafeTale.Infrastructure.Persistence;

public class RafeTaleDbContext(DbContextOptions<RafeTaleDbContext> options) : DbContext(options)
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterModifier> CharacterModifiers => Set<CharacterModifier>();
    public DbSet<CharacterInventory> CharacterInventories => Set<CharacterInventory>();
    public DbSet<CharacterStatus> CharacterStatuses => Set<CharacterStatus>();
    public DbSet<CharacterSpellSlots> CharacterSpellSlots => Set<CharacterSpellSlots>();
    public DbSet<ActiveModifiers> ActiveModifiers => Set<ActiveModifiers>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignCharacter> CampaignCharacters => Set<CampaignCharacter>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

    public DbSet<ClassDefinition> ClassDefinitions => Set<ClassDefinition>();
    public DbSet<ClassLevelProgression> ClassLevelProgressions => Set<ClassLevelProgression>();
    public DbSet<Subclass> Subclasses => Set<Subclass>();
    public DbSet<SubclassLevelProgression> SubclassLevelProgressions => Set<SubclassLevelProgression>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<Subrace> Subraces => Set<Subrace>();
    public DbSet<Background> Backgrounds => Set<Background>();
    public DbSet<Feat> Feats => Set<Feat>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Trait> Traits => Set<Trait>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<ItemTemplate> ItemTemplates => Set<ItemTemplate>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<XpRules> XpRules => Set<XpRules>();

    public DbSet<Rulebook> Rulebooks => Set<Rulebook>();
    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
    public DbSet<SkillDefinition> SkillDefinitions => Set<SkillDefinition>();
    public DbSet<DamageTypeDefinition> DamageTypeDefinitions => Set<DamageTypeDefinition>();
    public DbSet<ConditionDefinition> ConditionDefinitions => Set<ConditionDefinition>();
    public DbSet<LanguageDefinition> LanguageDefinitions => Set<LanguageDefinition>();
    public DbSet<CreatureTypeDefinition> CreatureTypeDefinitions => Set<CreatureTypeDefinition>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RafeTaleDbContext).Assembly);

        modelBuilder.Ignore<ModifierData>();
        modelBuilder.Ignore<FeatPrerequisiteModifierData>();
        modelBuilder.Ignore<ClassTrait>();
    }
}