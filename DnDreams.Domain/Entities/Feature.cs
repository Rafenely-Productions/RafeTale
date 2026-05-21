using DnDreams.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DnDreams.Domain.Entities
{
    public class Feature
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SpecialData { get; set; } = "";
        public bool RequiresChoice { get; set; } = false;

        public List<ModifierData> Modifiers { get; set; } = new();
    }
}
