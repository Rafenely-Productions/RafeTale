using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Entities.Rules;

public class CreatureTypeDefinition : IEntity
{
    public Guid Id { get; set; }      // "humanoid", "beast"
    public Guid RulebookId { get; set; }
    public string TechnicalName { get; set; } = string.Empty;

    public Rulebook Rulebook { get; set; } = null!;
}