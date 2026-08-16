using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence.Configurations
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
                .WithOne(e => e.Race)
                .HasForeignKey("RaceId");


            builder.HasMany(e => e.Traits)
                .WithOne(t => t.Race);

            builder.HasMany(e => e.Languages).WithMany(l => l.Races);
        }
    }
}