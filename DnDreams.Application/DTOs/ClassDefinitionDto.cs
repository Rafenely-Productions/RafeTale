using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.DTOs
{
    public class ClassDefinitionDto : IDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = "";
        public string HitDie { get; set; } = "d8";
        public int HitDieValue = 0;
        public ICollection<ASI> PrimaryAbility { get; set; } = new List<ASI>();
        public ICollection<ASI> SavingThrowProficiencies { get; set; } = new List<ASI>();
        public ICollection<ArmorProficiency> ArmorProficiencies { get; set; } = new List<ArmorProficiency>();
        public ICollection<WeaponProficiency> WeaponProficiencies { get; set; } = new List<WeaponProficiency>();
        public ICollection<ToolProficiency> ToolProficiencies { get; set; } = new List<ToolProficiency>();
        public List<Skill> SkillProficiencies { get; set; } = new List<Skill>();
        public int SkillToChoose { get; set; }


        public virtual ICollection<ClassLevelProgression> Progressions { get; set; } = new List<ClassLevelProgression>();
        public virtual ICollection<FeatureDto> FeatureDtos { get; set; } = new List<FeatureDto>();
        public virtual ICollection<SubclassDto> Subclasses { get; set; } = new List<SubclassDto>();

    }
}
