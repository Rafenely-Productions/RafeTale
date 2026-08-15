
namespace RafeTale.Application.DTOs
{
    public class ClassTraitDto
    {
        public string ResourceType { get; set; }

        public string Value { get; set; } = string.Empty;

        public int[] SpellSlots { get; set; } = new int[9];
    }
}
