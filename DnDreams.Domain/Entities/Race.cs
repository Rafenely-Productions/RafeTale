using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Entities
{
    public class Race
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        //public string Description { get; set; } = "";
        public string Resistances { get; set; } = "";
        public string Darkvision { get; set; } = "60 ft.";
        public SizeCategory Size { get; set; }
        public CreatureType CreatureType { get; set; }
        public string RacialTraits { get; set; } = "";

        public float Speed { get; set; } = 30;
        public List<LanguageType> Languages { get; set; } = new();
        public List<SubRace> SubRaces { get; set; } = new();

        public List<Trait> Traits { get; set; } = new();
        public Dictionary<string, int> StatBonuses { get; set; } = new();

    }
}
