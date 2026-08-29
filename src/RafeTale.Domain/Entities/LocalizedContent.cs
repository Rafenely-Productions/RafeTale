using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Domain.Entities
{
    public class LocalizedContent : IEntity
    {
        public Guid Id { get; set; }
        public LocEntity EntityType { get; set; }
        public Guid EntityId { get; set; }


        public LocProperty Property { get; set; }

        // El código del idioma (ISO 2 letras: "es", "en", "pt")
        public LocLanguage LanguageCode { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
