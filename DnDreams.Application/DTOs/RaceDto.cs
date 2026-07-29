using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;

namespace DnDreams.Application.DTOs
{
    public class RaceDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty; // Nombre traducido
        public string Description { get; init; } = string.Empty; // Lore traducido en HTML
        public string Resistances { get; init; } = string.Empty;
        public string Darkvision { get; init; } = "60 ft.";
        public SizeCategory Size { get; init; }
        public CreatureType CreatureType { get; init; }
        public string Speed { get; init; } = string.Empty;

        public List<Language> Languages { get; init; } = new();
        public List<SubRaceDto> SubRaces { get; init; } = new(); // Desacoplado con DTO
        public List<TraitDto> Traits { get; init; } = new(); // Desacoplado con DTO
    }
}