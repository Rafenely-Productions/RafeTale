
namespace RafeTale.Application.DTOs
{
    public class SubRaceDto
    {
        public Guid Id { get; init; }
        public string TechnicalName { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty; // Traducción del nombre de la subraza
        public string Description { get; init; } = string.Empty; // Traducción del lore/descripción
        public Guid RaceId { get; init; }
        public List<TraitDto> Traits { get; init; } = new();
    }
}