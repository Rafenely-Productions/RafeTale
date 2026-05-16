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

        public Dictionary<TargetPropertyType, string> SelectedAsi { get; set; } = new()
        {
            { TargetPropertyType.Strength, TargetPropertyType.Strength.ToString() },
            { TargetPropertyType.Dexterity, TargetPropertyType.Dexterity.ToString() },
            { TargetPropertyType.Constitution, TargetPropertyType.Constitution.ToString() },
            { TargetPropertyType.Intelligence, TargetPropertyType.Intelligence.ToString() },
            { TargetPropertyType.Wisdom, TargetPropertyType.Wisdom.ToString() },
            { TargetPropertyType.Charisma, TargetPropertyType.Charisma.ToString() }
        };

        public List<CharacterModifier> GetModifiersFromAsi()
        {
            return SelectedAsi
                .Select(kv => new CharacterModifier
                {
                    Source = "Mejora de Característica",
                    Type = ModifierType.AttributeBonus,
                    Target = kv.Key.ToString(),
                    Value = 0
                })
                .ToList();
        }
    }
}
