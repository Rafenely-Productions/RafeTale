using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Models
{
    public class LevelUpState
    {
        public int SelectedHp { get; set; }
        public List<CharacterModifier> SelectedModifiers { get; set; } = new();
        public List<Guid> SelectedFeatIds { get; set; } = new();
        public List<Guid> SelectedSpellIds { get; set; } = new();
        public Guid? SelectedFeatId { get; set; }

        public Dictionary<TargetPropertyType, int> SelectedAsi { get; set; } = new()
        {
            { TargetPropertyType.Strength, 0 },
            { TargetPropertyType.Dexterity, 0 },
            { TargetPropertyType.Constitution, 0 },
            { TargetPropertyType.Intelligence, 0 },
            { TargetPropertyType.Wisdom, 0 },
            { TargetPropertyType.Charisma, 0 }
        };

        public List<CharacterModifier> GetModifiersFromAsi()
        {
            return SelectedAsi
                .Where(kv => kv.Value > 0)
                .Select(kv => new CharacterModifier
                {
                    Source = "Mejora de Característica",
                    Type = ModifierType.AttributeBonus,
                    Target = kv.Key,
                    Value = kv.Value
                })
                .ToList();
        }
    }
}
