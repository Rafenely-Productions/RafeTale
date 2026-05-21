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
    public class ClassDefinitionConfiguration : IEntityTypeConfiguration<ClassDefinition>
    {
        public void Configure(EntityTypeBuilder<ClassDefinition> builder)
        {
            builder.ToTable("ClassDefinitions");
            builder.HasMany(cd => cd.Progressions)
                .WithOne();
        }
    }
}
