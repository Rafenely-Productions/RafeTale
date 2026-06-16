using DnDreams.Domain.Entities;
using DnDreams.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.DTOs
{
    public class FeatureDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = "";
        public string Special { get; init; } = "";
        public List<ModifierData> Modifiers { get; set; } = new();
    }
}
