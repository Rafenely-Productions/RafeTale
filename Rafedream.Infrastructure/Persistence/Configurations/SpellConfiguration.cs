using Rafedream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json; 

namespace Rafedream.Infrastructure.Persistence.Configurations
{
    public class SpellConfiguration : IEntityTypeConfiguration<Spell>
    {
        public void Configure(EntityTypeBuilder<Spell> builder)
        {
            builder.ToTable("Spells");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.TechnicalName)
                .IsRequired()
                .HasMaxLength(100);

           //TODO builder.Property(s => s.Description)
                //.HasColumnType("TEXT");

            builder.Property(s => s.School)
                .HasConversion<string>();

            builder.Property(s => s.ClassesTechnicalNames)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>())
            .HasColumnType("TEXT");
        }
        }
}
