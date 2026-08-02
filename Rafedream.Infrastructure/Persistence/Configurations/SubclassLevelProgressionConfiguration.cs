using Rafedream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Infrastructure.Persistence.Configurations
{
    public class SubclassLevelProgressionConfiguration : IEntityTypeConfiguration<SubclassLevelProgression>
    {
        public void Configure(EntityTypeBuilder<SubclassLevelProgression> builder)
        {
            builder.HasKey(e => e.Id);

            // Una Clase tiene muchas progresiones de nivel
            builder.HasOne(d => d.Subclass)
                  .WithMany(p => p.Progressions)
                  .HasForeignKey(d => d.SubclassId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Features)
              .WithOne();
        }
    }
}
