using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.DTOs
{
    public class RaceDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = "";
        public string Resistances { get; init; } = "";
        public string Darkvision { get; set; } = "60 ft.";
        public SizeCategory Size { get; set; }
        public CreatureType CreatureType { get; set; }

        public float Speed { get; set; } = 30;
        public List<Language> Languages { get; set; } = new();
        public List<SubRace> SubRaces { get; set; } = new();

        public List<Trait> Traits { get; set; } = new();
    }
}
