using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Modifiers
{
    public class ModifierData
    {
        public ModifierType Type { get; set; }
        public string Target { get; set; } = ""; // "Charisma", "MaxHp", "Spell_Shield"
        public int Value { get; set; }                     // 1, 2, 10, etc.
    }
}
