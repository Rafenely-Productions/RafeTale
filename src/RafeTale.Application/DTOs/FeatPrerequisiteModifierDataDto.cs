
namespace RafeTale.Application.DTOs
{
    public class FeatPrerequisiteModifierDataDto
    {
        public string Type { get; set; }
        public string Target { get; set; } = ""; // "Charisma", "MaxHp", "Spell_Shield"
        public int Value { get; set; }
    }
}
