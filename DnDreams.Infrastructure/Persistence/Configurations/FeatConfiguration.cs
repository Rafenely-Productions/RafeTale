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
            builder.HasMany(e => e.Modifiers);
            builder.HasMany(e => e.Prerequisite);

            builder.Ignore(e => e.Prerequisite);
            builder.Ignore(e => e.Modifiers);
        }
    }
}
