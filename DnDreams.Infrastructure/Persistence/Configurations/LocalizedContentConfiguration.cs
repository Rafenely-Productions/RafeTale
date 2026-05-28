using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence.Configurations
{
    public class LocalizedContentConfiguration : IEntityTypeConfiguration<LocalizedContent>
    {
        public void Configure(EntityTypeBuilder<LocalizedContent> builder)
        {
            builder.ToTable("LocalizedContents");
            builder.HasKey(e => e.Id);
            builder.HasIndex(x => new { x.EntityId, x.Property, x.LanguageCode }).IsUnique();
        }
    }
}
