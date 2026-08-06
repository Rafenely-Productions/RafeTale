using RafeTale.Domain.Entities;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence.Configurations
{
    public class CharacterModifierConfiguration : IEntityTypeConfiguration<CharacterModifier>
    {
        public void Configure(EntityTypeBuilder<CharacterModifier> builder)
        {
            builder.ToTable("CharacterModifiers");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
