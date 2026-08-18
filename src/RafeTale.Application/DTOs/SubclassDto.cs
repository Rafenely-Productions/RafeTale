using RafeTale.Application.Interfaces.DtosInterfaces;

namespace RafeTale.Application.DTOs
{
    public class SubclassDto :IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<SubclassLevelProgressionDto> Progressions { get; set; } = [];
        public virtual ICollection<FeatureDto> FeatureDtos { get; set; } = [];
    }
}
