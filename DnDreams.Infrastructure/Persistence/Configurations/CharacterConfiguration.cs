using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json;

namespace DnDreams.Infrastructure.Persistence.Configurations
{
    public class CharacterConfiguration : IEntityTypeConfiguration<Character>
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = false };

        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.ToTable("Characters");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

            // Relaciones Directas con Claves Foráneas Explícitas (Protección estricta de FKs)
            builder.HasOne(c => c.Race)
                   .WithMany()
                   .HasForeignKey(c => c.RaceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ClassDef)
                   .WithMany()
                   .HasForeignKey(c => c.ClassDefId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Background)
                   .WithMany()
                   .HasForeignKey(c => c.BackgroundId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Colecciones y Tablas Intermedias relacionales puras
            builder.HasMany(c => c.ClassLevels).WithOne();
            builder.HasMany(c => c.AcquiredFeatures).WithMany();
            builder.HasMany(c => c.AcquiredFeats).WithMany();
            builder.HasMany(c => c.KnownSpells).WithMany();

            builder.HasMany(c => c.CharacterModifiers)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);

            // 🚨 NOTA: NO configuramos aquí la relación HasMany(c => c.SpellSlots).
            // EF Core la unirá automáticamente usando la configuración explícita que ya tienes
            // declarada en tu otra clase 'CharacterSpellSlotsConfiguration'.

            // MAGIA: Convertir el Diccionario de Stats a un string JSON en la base de datos
            builder.Property(e => e.Stats)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, jsonOptions) ?? new Dictionary<string, int>()
                )
                .HasColumnType("TEXT");
        }
    }
}