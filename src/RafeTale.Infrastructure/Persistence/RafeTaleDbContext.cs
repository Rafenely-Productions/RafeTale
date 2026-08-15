using RafeTale.Domain.Entities;
using RafeTale.Domain.Modifiers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace RafeTale.Infrastructure.Persistence;

public class RafeTaleDbContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterModifier> CharacterModifiers => Set<CharacterModifier>();

    public DbSet<ClassDefinition> ClassDefinitions=> Set<ClassDefinition>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<ClassLevelProgression> ClassLevelProgressions => Set<ClassLevelProgression>();
    public DbSet<XpRules> XpRules => Set<XpRules>();
    public DbSet<Feat> Feats => Set<Feat>();
    public DbSet<Background> Backgrounds => Set<Background>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<ItemTemplate> ItemTemplates => Set<ItemTemplate>();
    public DbSet<CharacterInventory> CharacterInventories => Set<CharacterInventory>();
    public DbSet<CharacterStatus> CharacterStatuses => Set<CharacterStatus>();
    public DbSet<CharacterSpellSlots> CharacterSpellSlots => Set<CharacterSpellSlots>();
    public DbSet<ActiveModifiers> ActiveModifiers => Set<ActiveModifiers>();
    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<CampaignCharacter> CampaignCharacters { get; set; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
    public DbSet<SubRace> SubRaces { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<Trait> Traits { get; set; } = null!;
    public DbSet<Subclass> Subclasses { get; set; } = null!;
    public DbSet<SubclassLevelProgression> SubclassLevelProgressions { get; set; } = null!;
    public RafeTaleDbContext(DbContextOptions<RafeTaleDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RafeTaleDbContext).Assembly);

        modelBuilder.Ignore<ModifierData>();
        modelBuilder.Ignore<FeatPrerequisiteModifierData>();
        modelBuilder.Ignore<ClassTrait>();
    }
}