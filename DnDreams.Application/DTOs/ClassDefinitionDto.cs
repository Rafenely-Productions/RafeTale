using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
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
        public virtual ICollection<ClassLevelProgression> Progressions { get; set; } = new List<ClassLevelProgression>();
        public virtual ICollection<FeatureDto> FeatureDtos { get; set; } = new List<FeatureDto>();

    }
}
