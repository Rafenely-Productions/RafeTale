using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Domain.Entities
{
    public class SpecialTrait : IEntity
    {
        public Guid Id { get; set; }
        public string TechnicalName { get; set; } = string.Empty;
        public List<ModifierData> Modifiers { get; set; } = new();


        // Relación con la SubRaza
        public Guid TraitId { get; set; }
        public Trait Trait { get; set; } = null!;
    }
}
