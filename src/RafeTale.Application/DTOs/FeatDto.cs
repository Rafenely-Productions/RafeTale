using RafeTale.Application.Interfaces.DtosInterfaces;

namespace RafeTale.Application.DTOs
{
    public class FeatDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<FeatPrerequisiteModifierDataDto> Prerequisite { get; set; } = [];
        public List<ModifierDataDto> Modifiers { get; set; } = [];
    }
}
