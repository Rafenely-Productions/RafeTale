using RafeTale.Application.Interfaces.DtosInterfaces;

namespace RafeTale.Application.DTOs
{
    public class RaceDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // Nombre traducido
        public string Description { get; set; } = string.Empty; // Lore traducido en HTML
        public string Resistances { get; init; } = string.Empty;
        public string Darkvision { get; init; } = "60 ft.";
        public string Size { get; init; } = string.Empty;
        public string CreatureType { get; init; } = string.Empty;
        public string Speed { get; init; } = string.Empty;

        public List<LanguageDto> Languages { get; init; } = [];
        public List<SubRaceDto> SubRaces { get; init; } = [];
        public List<TraitDto> Traits { get; init; } = [];
    }
}