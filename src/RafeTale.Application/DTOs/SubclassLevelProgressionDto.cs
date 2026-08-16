
namespace RafeTale.Application.DTOs
{
    public class SubclassLevelProgressionDto
    {
        public Guid Id { get; set; }
        public int Level { get; set; }
        public List<FeatureDto> Features { get; set; } = new(); // Lo que ganas en este nivel
        public SubclassDto? Subclass { get; set; }
        public Guid SubclassId { get; set; }
    }
}
