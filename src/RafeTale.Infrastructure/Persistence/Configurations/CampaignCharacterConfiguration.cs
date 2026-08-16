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
    public class CampaignCharacterConfiguratio : IEntityTypeConfiguration<CampaignCharacter>
    {
        public void Configure(EntityTypeBuilder<CampaignCharacter> builder)
        {
            builder.ToTable("CampaignCharacters");

            builder.HasKey(e => e.Id);

            // Relación con Campaña: Si se borra la campaña, se limpia la intermedia
            builder.HasOne(d => d.Campaign)
                .WithMany(p => p.CampaignCharacters)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Personaje: Si se borra el personaje, se limpia la intermedia
            builder.HasOne(d => d.Character)
                .WithMany(p => p.CampaignCharacters)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
