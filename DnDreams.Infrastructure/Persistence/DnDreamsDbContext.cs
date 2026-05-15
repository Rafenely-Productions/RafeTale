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

    public DnDreamsDbContext(DbContextOptions<DnDreamsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var jsonOptions = new JsonSerializerOptions { WriteIndented = false };

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
            entity.HasMany(c => c.ActiveModifiers).WithOne().OnDelete(DeleteBehavior.Cascade);
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
    }
}