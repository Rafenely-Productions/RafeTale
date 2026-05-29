using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
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
    public class RaceConfiguration : IEntityTypeConfiguration<Race>
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = false };

        public void Configure(EntityTypeBuilder<Race> builder)
        {
            builder.ToTable("Races");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.TechnicalName).HasMaxLength(50);

            builder.HasMany(e => e.SubRaces)
                .WithOne(e=> e.Race)
                .HasForeignKey(sr => sr.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Traits)
                .WithOne(t => t.Race)
                .HasForeignKey(t => t.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Languages).WithMany(l => l.Races);

            builder.Property(e => e.StatBonuses)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, jsonOptions) ?? new Dictionary<string, int>()
                )
                .HasColumnType("TEXT");
        }
    }
}