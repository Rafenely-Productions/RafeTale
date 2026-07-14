using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.DTOs
{
    public class FeatDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = "";
        public CategoryFeat Category { get; set; }
        bool SpecialData = false;
        public List<FeatPrerequisiteModifierData> Prerequisite { get; set; } = [];
        public List<ModifierData> Modifiers { get; set; } = [];
    }
}
