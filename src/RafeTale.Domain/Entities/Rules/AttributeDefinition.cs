namespace RafeTale.Domain.Entities.Rules;

public class AttributeDefinition
{
    public string Id { get; set; } = string.Empty;           // "str", "dex", "san"
    public string RulebookId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;         // "Strength"
    public string ShortName { get; set; } = string.Empty;    // "STR"
    public int DefaultMin { get; set; } = 1;
    public int DefaultMax { get; set; } = 20;
    public int DisplayOrder { get; set; }
}

public class SkillDefinition
{
    public string Id { get; set; } = string.Empty;           // "athletics", "stealth"
    public string RulebookId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AttributeId { get; set; } = string.Empty;  // Relación a "str"
    public string? Description { get; set; }
}

public class DamageTypeDefinition
{
    public string Id { get; set; } = string.Empty;           // "fire", "slashing"
    public string RulebookId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ConditionDefinition
{
    public string Id { get; set; } = string.Empty;           // "blinded", "prone"
    public string RulebookId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class LanguageDefinition
{
    public string Id { get; set; } = string.Empty;           // "common", "elvish"
    public string RulebookId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Script { get; set; }                      // "Common", "Elven"
}