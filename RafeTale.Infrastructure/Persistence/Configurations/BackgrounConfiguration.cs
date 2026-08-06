using RafeTale.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence.Configurations
{
    public class BackgrounConfiguration
    {
        public void Configure(EntityTypeBuilder<Background> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.TechnicalName).IsRequired().HasMaxLength(100);
            builder.HasOne(e => e.Feat);

        }
    }
}
