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
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.ToTable("JournalEntries");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Content).IsRequired();

            // Relación opcional con Campaña (Una campaña tiene muchos registros de diario)
            builder.HasOne(d => d.Campaign)
                .WithMany() // Si en el futuro quieres un List<JournalEntry> en Campaign, lo pones aquí
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación opcional con Personaje (Un personaje puede tener sus notas privadas)
            builder.HasOne(d => d.Character)
                .WithMany() // Si en el futuro quieres un List<JournalEntry> en Character, lo pones aquí
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
