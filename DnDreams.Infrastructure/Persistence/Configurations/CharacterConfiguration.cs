using DnDreams.Domain.Entities;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            builder.HasMany(c => c.ClassLevels).WithOne();
            builder.HasMany(c => c.AcquiredFeatures).WithMany();

            // MAGIA: Convertir el Diccionario de Stats a un string JSON en la base de datos
            builder.Property(e => e.Stats)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions), // Cómo se guarda (Dict -> string)
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, jsonOptions) ?? new Dictionary<string, int>() // Cómo se lee (string -> Dict)
                )
                .HasColumnType("TEXT"); // SQLite lo guardará en una columna de tipo texto

            builder.HasMany(c => c.AcquiredFeats).WithMany();
            builder.HasMany(c => c.KnownSpells).WithMany();
            builder.HasMany(c => c.CharacterModifiers).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
