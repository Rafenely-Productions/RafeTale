using Rafedream.Domain.Enums;
using Rafedream.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Domain.Entities
{
    public class Race : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TechnicalName { get; set; } = string.Empty;
        public SizeCategory Size { get; set; }
        public CreatureType CreatureType { get; set; }

        public string Speed { get; set; } = string.Empty;
        public List<Language> Languages { get; set; } = new();
        public List<SubRace> SubRaces { get; set; } = new();

        public List<Trait> Traits { get; set; } = new();
    }
}
