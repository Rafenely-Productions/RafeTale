
namespace RafeTale.Application.DTOs
{
    public class FeatPrerequisiteModifierDataDto
    {
        public string Type { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty; // "Charisma", "MaxHp", "Spell_Shield"
        public int Value { get; set; }
    }
}
