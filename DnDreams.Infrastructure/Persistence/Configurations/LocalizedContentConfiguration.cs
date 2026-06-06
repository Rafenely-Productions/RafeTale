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

            // Índices y restricciones
            builder.Property(x => x.EntityId).IsRequired();
            builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Property).HasMaxLength(100).IsRequired();

            // El texto de la traducción sí puede ser largo
            builder.Property(x => x.Text).HasColumnType("TEXT").IsRequired();

            // Índice compuesto ÚNICO: La combinación de estas 3 cosas debe ser irrepetible
            builder.HasIndex(x => new { x.EntityId, x.Property, x.LanguageCode })
                   .HasDatabaseName("IX_LocalizedContent_Lookup")
                   .IsUnique();
        }
    }
}
