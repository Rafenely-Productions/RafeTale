using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Entities.Rules;

public class DamageTypeDefinition : IEntity
{
    public Guid Id { get; set; }      // "fire", "slashing"
    public Guid RulebookId { get; set; }
    public string TechnicalName { get; set; } = string.Empty;

    public Rulebook Rulebook { get; set; } = null!;
}
