using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DnDreams.Infrastructure.Persistence;

public class DnDreamsDbContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<ClassDefinition> ClassDefinitions { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Race> Races => Set<Race>();
    public DbSet<ClassLevelProgression> ClassLevelProgressions => Set<ClassLevelProgression>();
    public DbSet<XpRules> XpRules => Set<XpRules>();
    public DbSet<Feat> Feats => Set<Feat>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<CharacterModifier> characterModifiers => Set<CharacterModifier>();
    public DbSet<ItemTemplate> ItemTemplates { get; set; }
    public DbSet<CharacterInventory> CharacterInventories { get; set; }
    public DbSet<CharacterStatus> CharacterStatuses { get; set; } = null!;
    public DbSet<CharacterSpellSlots> CharacterSpellSlots { get; set; } = null!;
    public DbSet<ActiveModifiers> ActiveModifiers { get; set; } = null!;
    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<CampaignCharacter> CampaignCharacters { get; set; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;

    public DnDreamsDbContext(DbContextOptions<DnDreamsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var jsonOptions = new JsonSerializerOptions { WriteIndented = false };

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.DungeonMasterName).IsRequired();
        });

        // 2. Tabla Intermedia (Muchos a Muchos)
        modelBuilder.Entity<CampaignCharacter>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relación con Campaña: Si se borra la campaña, se limpia la intermedia
            entity.HasOne(d => d.Campaign)
                .WithMany(p => p.CampaignCharacters)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Personaje: Si se borra el personaje, se limpia la intermedia
            entity.HasOne(d => d.Character)
                .WithMany(p => p.CampaignCharacters)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterStatus>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relación 1 a 1: Un Personaje tiene UN Solo Estado Vital, y el Estado pertenece a UN Personaje.
            // Al borrar el personaje, se borra su status en cascada.
            entity.HasOne(d => d.Character)
                .WithOne(p => p.Status)
                .HasForeignKey<CharacterStatus>(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mapeo del Enum Flag como entero para SQLite
            entity.Property(e => e.ActiveConditions)
                .HasConversion<int>();
        });
        modelBuilder.Entity<ActiveModifiers>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relación 1 a Muchos: Un personaje puede verse afectado por múltiples modificadores vivos
            entity.HasOne(d => d.Character)
                .WithMany(p => p.ActiveModifiers)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Guardar los Enums como strings en la DB para que sea legible al depurar SQLite
            entity.Property(e => e.TargetProperty)
                .HasConversion<string>();

            entity.Property(e => e.DurationType)
                .HasConversion<string>();
        });
        modelBuilder.Entity<CharacterSpellSlots>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relación 1 a Muchos: Un personaje puede tener varios niveles de Slots
            entity.HasOne(d => d.Character)
                .WithMany(p => p.SpellSlots)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Feat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiersJson).HasColumnType("TEXT");

            entity.Ignore(e => e.Modifiers);
        });
        modelBuilder.Entity<Spell>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasMany(c => c.ClassLevels).WithOne();
            entity.HasMany(c => c.AcquiredFeatures).WithMany();

            // MAGIA: Convertir el Diccionario de Stats a un string JSON en la base de datos
            entity.Property(e => e.Stats)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions), // Cómo se guarda (Dict -> string)
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, jsonOptions) ?? new Dictionary<string, int>() // Cómo se lee (string -> Dict)
                )
                .HasColumnType("TEXT"); // SQLite lo guardará en una columna de tipo texto

            entity.HasMany(c => c.AcquiredFeats).WithMany();
            entity.HasMany(c => c.KnownSpells).WithMany();
            entity.HasMany(c => c.CharacterModifiers).WithOne().OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);

            // MAGIA: Convertir los bonos raciales a JSON
            entity.Property(e => e.StatBonuses)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, jsonOptions) ?? new Dictionary<string, int>()
                )
                .HasColumnType("TEXT");
        });

        // Las clases tienen una progresión de niveles.
        modelBuilder.Entity<ClassDefinition>()
            .HasMany(cd => cd.Progressions)
            .WithOne();

        modelBuilder.Entity<ClassLevelProgression>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Una Clase tiene muchas progresiones de nivel
            entity.HasOne(d => d.ClassDef)
                  .WithMany(p => p.Progressions)
                  .HasForeignKey(d => d.ClassDefId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Features)
              .WithOne();
        });
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModifiersJson).HasColumnType("TEXT");

            entity.Ignore(e => e.Modifiers);
        });
        modelBuilder.Entity<XpRules>()
            .HasKey(e => e.Level);
        modelBuilder.Entity<CharacterModifier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // <-- Asegúrate de que tenga esto
        });

        modelBuilder.Entity<CharacterInventory>()
        .HasOne(ci => ci.Item)
        .WithMany()
        .HasForeignKey(ci => ci.ItemTemplateId);

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();

            // Relación opcional con Campaña (Una campaña tiene muchos registros de diario)
            entity.HasOne(d => d.Campaign)
                .WithMany() // Si en el futuro quieres un List<JournalEntry> en Campaign, lo pones aquí
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación opcional con Personaje (Un personaje puede tener sus notas privadas)
            entity.HasOne(d => d.Character)
                .WithMany() // Si en el futuro quieres un List<JournalEntry> en Character, lo pones aquí
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}