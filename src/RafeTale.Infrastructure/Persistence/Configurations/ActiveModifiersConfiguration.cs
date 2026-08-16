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
    public class ActiveModifiersConfiguration : IEntityTypeConfiguration<ActiveModifiers>
    {
        public void Configure(EntityTypeBuilder<ActiveModifiers> builder)
        {
            builder.ToTable("ActiveModifiers");

            builder.HasKey(e => e.Id);

            // Relación 1 a Muchos: Un personaje puede verse afectado por múltiples modificadores vivos
            builder.HasOne(d => d.Character)
                .WithMany(p => p.ActiveModifiers)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Guardar los Enums como strings en la DB para que sea legible al depurar SQLite
            builder.Property(e => e.TargetProperty)
                .HasConversion<string>();

            builder.Property(e => e.DurationType)
                .HasConversion<string>();
        }
    }
}
