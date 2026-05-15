using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Modifiers
{
    public class ModifierData
    {
        public string Type { get; set; } = string.Empty;   // "AttributeBonus", "GrantSpell", etc.
        public string Target { get; set; } = string.Empty; // "Charisma", "MaxHp", "Spell_Shield"
        public int Value { get; set; }                     // 1, 2, 10, etc.
    }
}
