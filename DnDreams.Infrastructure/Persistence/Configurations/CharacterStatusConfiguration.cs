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
    public class CharacterStatusConfiguration : IEntityTypeConfiguration<CharacterStatus>
    {
        public void Configure(EntityTypeBuilder<CharacterStatus> builder)
        {
            builder.ToTable("CharacterStatus");

            builder.HasKey(e => e.Id);

            // Relación 1 a 1: Un Personaje tiene UN Solo Estado Vital, y el Estado pertenece a UN Personaje.
            // Al borrar el personaje, se borra su status en cascada.
            builder.HasOne(d => d.Character)
                .WithOne(p => p.Status)
                .HasForeignKey<CharacterStatus>(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mapeo del Enum Flag como entero para SQLite
            builder.Property(e => e.ActiveConditions)
                .HasConversion<int>();
        }
    }
}
