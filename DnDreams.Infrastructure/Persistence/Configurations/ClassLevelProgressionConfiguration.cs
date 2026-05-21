using DnDreams.Domain.Entities;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence.Configurations
{
    public class ClassLevelProgressionConfiguration : IEntityTypeConfiguration<ClassLevelProgression>
    {
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
        }
    }
}
