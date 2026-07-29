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
    public class SubclassConfiguration : IEntityTypeConfiguration<Subclass>
    {
        public void Configure(EntityTypeBuilder<Subclass> builder)
        {
            builder.ToTable("Subclasses");
            builder.HasKey(sr => sr.Id);
            builder.Property(sr => sr.TechnicalName).IsRequired().HasMaxLength(50);

            builder.HasOne(s => s.ClassDefinition)
            .WithMany(c => c.Subclasses)
            .HasForeignKey(s => s.ClassDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cd => cd.Progressions)
                .WithOne()
                .HasForeignKey(p => p.SubclassId) 
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
