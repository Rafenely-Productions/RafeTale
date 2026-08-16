using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RafeTale.Domain.Entities
{
    public class Feature : IEntity
    {
        public Guid Id { get; set; }
        public string TechnicalName { get; set; } = string.Empty;
        public bool RequiresChoice { get; set; } = false;

        public List<ModifierData> Modifiers { get; set; } = new();
    }
}
