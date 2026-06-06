using DnDreams.Domain.Entities;
using DnDreams.Domain.Modifiers;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence.Configurations
{
    public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable("Features");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.TechnicalName).IsRequired().HasMaxLength(100);
            //builder.HasMany(e => e.Modifiers);

                builder.Property(f => f.Modifiers)
                    .HasConversion(
                        // Al guardar en la DB: Convertimos la Lista a string (JSON)
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        // Al leer de la DB: Convertimos el string (JSON) de vuelta a Lista
                        v => JsonSerializer.Deserialize<List<ModifierData>>(v, (JsonSerializerOptions)null)
                             ?? new List<ModifierData>()
                    );
        }
    }
}
