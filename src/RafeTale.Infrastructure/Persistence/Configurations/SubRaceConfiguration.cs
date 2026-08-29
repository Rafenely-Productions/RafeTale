using RafeTale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence.Configurations
{
    public class SubraceConfiguration : IEntityTypeConfiguration<Subrace>
    {
        public void Configure(EntityTypeBuilder<Subrace> builder)
        {
            builder.ToTable("Subraces");
            builder.HasKey(sr => sr.Id);
            builder.Property(sr => sr.TechnicalName).IsRequired().HasMaxLength(50);

            builder.HasOne(s => s.Race)
            .WithMany(c => c.Subraces)
            .HasForeignKey(s => s.RaceId)
            .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
