using DnDreams.Domain.Enums;
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

        public LocEntity EntityType { get; set; }

        // Qué campo estamos traduciendo (ej: "Name", "Description", "Lore")
        public LocProperty Property { get; set; }

        // El código del idioma (ISO 2 letras: "es", "en", "pt")
        public LocLanguage LanguageCode { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
