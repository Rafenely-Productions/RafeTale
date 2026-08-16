
using RafeTale.Domain.Enums;

namespace RafeTale.Application.DTOs
{
    public class ModifierDataDto
    {
        public ModifierTypeDto Type { get; set; }
        public string Target { get; set; } = ""; // "Charisma", "MaxHp", "Spell_Shield"
        public int Value { get; set; }
    }
}
