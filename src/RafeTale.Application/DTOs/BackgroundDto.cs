using RafeTale.Application.Interfaces.DtosInterfaces;

namespace RafeTale.Application.DTOs
{
    public class BackgroundDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // Propiedades localizadas que se inyectan ya traducidas
        public string TechnicalName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ToolProficiencies { get; set; } = string.Empty;
        public string Equipment { get; set; } = string.Empty;

        // Propiedades mecánicas nativas
        public ICollection<string> ASIs { get; set; } = [];
        public ICollection<string> SkillProficiencies { get; set; } = [];

        // Relación desacoplada con la Dote de origen de nivel 1
        public Guid? FeatId { get; set; }
        public FeatDto? Feat { get; set; }
    }
}