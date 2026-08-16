using RafeTale.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence.Configurations
{
    public class XpRulesConfiguration : IEntityTypeConfiguration<XpRules>
    {
        public void Configure(EntityTypeBuilder<XpRules> builder)
        {
            builder.ToTable("XpRules");
            builder.HasKey(e => e.Level);
        }
    }
}
