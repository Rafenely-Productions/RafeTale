using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Entities
{
    public class LocalizedContent : IEntity
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }

        public string EntityType { get; set; } = string.Empty;

        // Qué campo estamos traduciendo (ej: "Name", "Description", "Lore")
        public string Property { get; set; } = string.Empty;

        // El código del idioma (ISO 2 letras: "es", "en", "pt")
        public string LanguageCode { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
