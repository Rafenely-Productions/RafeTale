using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Entities.Rules;

public class CreatureSizeDefinition : IEntity
{
    public Guid Id { get; set; }      // "common", "elvish"
    public string RulebookId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Script { get; set; }                      // "Common", "Elven"

    public Rulebook Rulebook { get; set; } = null!;
}