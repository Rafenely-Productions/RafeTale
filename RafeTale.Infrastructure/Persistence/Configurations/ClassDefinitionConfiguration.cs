using RafeTale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence.Configurations
{
    public class ClassDefinitionConfiguration : IEntityTypeConfiguration<ClassDefinition>
    {
        public void Configure(EntityTypeBuilder<ClassDefinition> builder)
        {
            builder.ToTable("ClassDefinitions");
            
            builder.HasMany(c => c.Subclasses)
                .WithOne()
                .HasForeignKey("ClassDefinitionId");
            
            builder.HasMany(c => c.SkillProficiencies)
                .WithMany()
                .UsingEntity(j => j.ToTable("ClassSkillProficiencies"));

            builder.HasMany(cd => cd.Progressions)
                .WithOne();
        }
    }
}
