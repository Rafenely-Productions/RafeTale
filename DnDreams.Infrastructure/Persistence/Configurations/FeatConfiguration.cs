using DnDreams.Domain.Entities;
using DnDreams.Domain.Modifiers;
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
    public class FeatConfiguration : IEntityTypeConfiguration<Feat>
    {

        public void Configure(EntityTypeBuilder<Feat> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.TechnicalName).IsRequired().HasMaxLength(100);
            builder.Property(f => f.Modifiers)
             .HasConversion(
                 // Al guardar
                 v => v == null ? "[]" : JsonSerializer.Serialize(v, (JsonSerializerOptions)null),

                 // Al leer (Protección contra strings vacíos o "no-JSON")
                 v => string.IsNullOrWhiteSpace(v)
                     ? new List<ModifierData>()
                     : JsonSerializer.Deserialize<List<ModifierData>>(v, (JsonSerializerOptions)null) ?? new List<ModifierData>()
             );
            builder.HasMany(e => e.Prerequisite);

            builder.Ignore(e => e.Prerequisite);
        }
    }
}
