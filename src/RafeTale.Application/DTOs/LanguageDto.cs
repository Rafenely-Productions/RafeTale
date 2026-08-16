

namespace RafeTale.Application.DTOs
{
    public class LanguageDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TechnicalName { get; set; } = string.Empty;
        public IEnumerable<RaceDto> Races { get; set; } = [];
    }
}
