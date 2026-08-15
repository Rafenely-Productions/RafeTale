
namespace RafeTale.Application.DTOs
{
    public class ClassLevelProgressionDto
    {
        public Guid Id { get; set; }
        public int Level { get; set; }
        public List<FeatureDto> Features { get; set; } = [];
        public List<ClassTraitDto> Traits = [];
        public ClassDefinitionDto? ClassDef { get; set; }
        public Guid ClassDefId { get; set; }
    }
}
