using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Entities.Rules;

public class SkillDefinition : IEntity
{
    public Guid Id { get; set; } 
    public Guid RulebookId { get; set; }
    public string TechnicalName { get; set; } = string.Empty;
    public Guid AttributeId { get; set; } 

    public Rulebook Rulebook { get; set; } = null!;
    public AttributeDefinition? Attribute { get; set; }
}
