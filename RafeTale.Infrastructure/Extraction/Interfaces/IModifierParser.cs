using RafeTale.Domain.Modifiers;

namespace RafeTale.Infrastructure.Extraction.Interfaces
{
    public interface IModifierParser
    {
        List<ModifierData> ParseModifiers(string json);
        List<FeatPrerequisiteModifierData> ParsePrerequisites(string json);
        List<ClassTrait> ParseClassTraits(string raw);
    }
}
