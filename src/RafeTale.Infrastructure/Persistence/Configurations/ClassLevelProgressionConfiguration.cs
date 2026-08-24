using RafeTale.Domain.Entities;
using RafeTale.Domain.Modifiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RafeTale.Infrastructure.Persistence.Configurations
{
    public class ClassLevelProgressionConfiguration : IEntityTypeConfiguration<ClassLevelProgression>
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true, // Ignora si en el JSON viene 'type' o 'Type'
        };
        public void Configure(EntityTypeBuilder<ClassLevelProgression> builder)
        {
            builder.HasKey(e => e.Id);

            // Una Clase tiene muchas progresiones de nivel
            builder.HasOne(d => d.ClassDef)
                  .WithMany(p => p.Progressions)
                  .HasForeignKey(d => d.ClassDefId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Features)
              .WithOne();
            builder.Property(e => e.Traits)
                   .HasConversion(
                       v => JsonSerializer.Serialize(v, _jsonOptions),
                       v => JsonSerializer.Deserialize<List<ClassTrait>>(v, _jsonOptions) ?? new List<ClassTrait>()
                   )
                   .HasColumnType("TEXT");
        }
    }
}
