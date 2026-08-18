using RafeTale.Application.Interfaces.DtosInterfaces;

namespace RafeTale.Application.DTOs
{
    public class FeatureDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Special { get; init; } = string.Empty;
        public List<ModifierDataDto> Modifiers { get; set; } = [];
    }
}
