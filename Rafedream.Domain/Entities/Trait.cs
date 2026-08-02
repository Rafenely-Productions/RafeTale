using Rafedream.Domain.Interfaces;
using Rafedream.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Domain.Entities
{
    public class Trait : IEntity
    {
        public Guid Id { get; set; }
        public string TechnicalName { get; set; } = string.Empty;
        public int RequiredLevel { get; set; } = 1;
        public List<ModifierData> Modifiers { get; set; } = new();

        public Guid? RaceId { get; set; }
        public Race? Race { get; set; }

        public Guid? SubraceId { get; set; }
        public SubRace? Subrace { get; set; }
    }
}
