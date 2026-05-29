using DnDreams.Domain.Entities;
using DnDreams.Domain.Modifiers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DnDreams.Infrastructure.Persistence;

public class DnDreamsDbContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterModifier> CharacterModifiers => Set<CharacterModifier>();

    public DbSet<ClassDefinition> ClassDefinitions { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Race> Races => Set<Race>();
    public DbSet<ClassLevelProgression> ClassLevelProgressions => Set<ClassLevelProgression>();
    public DbSet<XpRules> XpRules => Set<XpRules>();
    public DbSet<Feat> Feats => Set<Feat>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<ItemTemplate> ItemTemplates { get; set; }
    public DbSet<CharacterInventory> CharacterInventories { get; set; }
    public DbSet<CharacterStatus> CharacterStatuses { get; set; } = null!;
    public DbSet<CharacterSpellSlots> CharacterSpellSlots { get; set; } = null!;
    public DbSet<ActiveModifiers> ActiveModifiers { get; set; } = null!;
    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<CampaignCharacter> CampaignCharacters { get; set; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
    public DbSet<SubRace> SubRaces { get; set; } = null!;
    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<Trait> Traits { get; set; } = null!;

    public DnDreamsDbContext(DbContextOptions<DnDreamsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DnDreamsDbContext).Assembly);

        modelBuilder.Ignore<ModifierData>();
    }
}