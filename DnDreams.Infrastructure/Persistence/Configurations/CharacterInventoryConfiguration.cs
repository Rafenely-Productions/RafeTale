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
    public class CharacterInventoryConfiguration : IEntityTypeConfiguration<CharacterInventory>
    {
        public void Configure(EntityTypeBuilder<CharacterInventory> builder)
        {
            builder.ToTable("CharacterInventories");
            builder.HasOne(ci => ci.Item)
                .WithMany()
                .HasForeignKey(ci => ci.ItemTemplateId);
        }
    }
}
