using RafeTale.Domain.Enums;

namespace RafeTale.Domain.Entities.Rules;

public class Modifier
{
    public ModifierType Type { get; set; }                   // Sigue siendo Enum: AttributeBonus, ModifySpeed, GrantAdvantage...
    public string TargetKey { get; set; } = string.Empty;    // "str", "ac", "athletics", "speed", etc.
    public int Value { get; set; }
    public ModifierDurationType Duration { get; set; }       // Instantaneous, Rounds, UntilLongRest, Permanent
}