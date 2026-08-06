using RafeTale.Domain.Entities;
using RafeTale.Domain.Modifiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence.Configurations
{
    public class TraitConfiguration : IEntityTypeConfiguration<Trait>
    {
        public void Configure(EntityTypeBuilder<Trait> builder)
        {
            builder.ToTable("Traits");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.TechnicalName).IsRequired().HasMaxLength(100);

            builder.HasOne(s => s.Race)
               .WithMany(c => c.Traits)
               .HasForeignKey(s => s.RaceId)
               .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.Subrace)
                .WithMany(s => s.Traits) 
                .HasForeignKey(t => t.SubraceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.Modifiers)
            .HasConversion(
                // Al guardar: Si la lista es nula o no tiene elementos, guardamos un JSON vacío [] o null
                v => v == null ? "[]" : JsonSerializer.Serialize(v, (JsonSerializerOptions)null),

                // Al leer: Si la celda en SQLite está vacía, nula o blanca, regresamos una lista vacía para evitar el crash
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<ModifierData>()
                    : JsonSerializer.Deserialize<List<ModifierData>>(v, (JsonSerializerOptions)null) ?? new List<ModifierData>()
            );
        }
    }
}
