using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence.Configurations
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

            builder.HasMany(s => s.Classes);
        }
    }
}
