using DnDreams.Domain.Interfaces;
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
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int RequiredLevel { get; set; } = 1;

        // Relación con la Raza
        public Guid RaceId { get; set; }
        public Race Race { get; set; } = null!;
    }
}
