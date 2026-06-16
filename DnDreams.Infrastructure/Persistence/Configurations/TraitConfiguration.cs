using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence.Configurations
{
    public class TraitConfiguration : IEntityTypeConfiguration<Trait>
    {
        public void Configure(EntityTypeBuilder<Trait> builder)
        {
            builder.ToTable("Traits");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.TechnicalName).IsRequired().HasMaxLength(100);
        }
    }
}
