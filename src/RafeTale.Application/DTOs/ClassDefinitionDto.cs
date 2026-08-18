using RafeTale.Application.Interfaces.DtosInterfaces;

namespace RafeTale.Application.DTOs
{
    public class ClassDefinitionDto : IDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TechnicalName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HitDie { get; set; } = "d8";
        public int HitDieValue = 0;
        public ICollection<string> PrimaryAbility { get; set; } = [];
        public ICollection<string> SavingThrowProficiencies { get; set; } = [];
        public ICollection<string> ArmorProficiencies { get; set; } = [];
        public ICollection<string> WeaponProficiencies { get; set; } = [];
        public ICollection<string> ToolProficiencies { get; set; } = [];
        public List<string?> SkillProficiencies { get; set; } = [];
        public int SkillToChoose { get; set; }


        public virtual ICollection<ClassLevelProgressionDto> Progressions { get; set; } = [];
        public virtual ICollection<FeatureDto> FeatureDtos { get; set; } = [];
        public virtual ICollection<SubclassDto> Subclasses { get; set; } = [];

    }
}
