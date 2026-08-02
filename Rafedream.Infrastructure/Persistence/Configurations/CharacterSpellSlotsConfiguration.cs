using Rafedream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rafedream.Infrastructure.Persistence.Configurations
{
    public class CharacterSpellSlotsConfiguration : IEntityTypeConfiguration<CharacterSpellSlots>
    {
        public void Configure(EntityTypeBuilder<CharacterSpellSlots> builder)
        {
            builder.ToTable("CharacterSpellSlots");

            builder.HasKey(e => e.Id);


            builder.HasOne(d => d.Character)
                .WithMany(p => p.SpellSlots)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
