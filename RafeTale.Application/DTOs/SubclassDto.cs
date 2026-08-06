using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Application.DTOs
{
    public class SubclassDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = "";

        public virtual ICollection<SubclassLevelProgression> Progressions { get; set; } = new List<SubclassLevelProgression>();
        public virtual ICollection<FeatureDto> FeatureDtos { get; set; } = new List<FeatureDto>();
    }
}
