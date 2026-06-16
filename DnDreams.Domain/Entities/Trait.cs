using DnDreams.Domain.Interfaces;
using DnDreams.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Entities
{
    public class Trait : IEntity
    {
        public Guid Id { get; set; }
        public string TechnicalName { get; set; } = string.Empty;
        public int RequiredLevel { get; set; } = 1;
        public List<ModifierData> Modifiers { get; set; } = new();

        // Relación con la Raza
        public Guid RaceId { get; set; }
        public Race Race { get; set; } = null!;
    }
}
