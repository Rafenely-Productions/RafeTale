using DnDreams.Domain.Entities;
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

        public Dictionary<string, int> SelectedAsi { get; set; } = new()
    {
        { "Strength", 0 }, { "Dexterity", 0 }, { "Constitution", 0 },
        { "Intelligence", 0 }, { "Wisdom", 0 }, { "Charisma", 0 }
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
