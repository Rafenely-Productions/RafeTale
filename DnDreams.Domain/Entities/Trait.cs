using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Entities
{
    public class Trait
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Relación con la Raza
        public Guid RaceId { get; set; }
    }
}
