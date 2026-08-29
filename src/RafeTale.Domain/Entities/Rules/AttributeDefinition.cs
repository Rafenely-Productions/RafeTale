using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Entities.Rules;

public class AttributeDefinition : IEntity
{
    public Guid Id { get; set; }      // "str", "dex", "san"
    public Guid RulebookId { get; set; }
    public string TechnicalName { get; set; } = string.Empty;         // "Strength"
    public string Name { get; set; } = string.Empty;    // "STR"
    public int DefaultMin { get; set; } = 1;
    public int DefaultMax { get; set; } = 20;
    public int DisplayOrder { get; set; }

    public Rulebook Rulebook { get; set; } = null!;
}
