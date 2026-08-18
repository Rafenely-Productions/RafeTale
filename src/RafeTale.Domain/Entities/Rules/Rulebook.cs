namespace RafeTale.Domain.Entities.Rules;

public class Rulebook
{
    public string Id { get; set; } = string.Empty;           // ej: "dnd-5e-srd"
    public string Name { get; set; } = string.Empty;         // "D&D 5e SRD"
    public string? SystemId { get; set; }                    // null si es base, o "dnd-5e" si es expansión
    public string Version { get; set; } = "1.0.0";
    public string Author { get; set; } = "RafeTale";
    public bool IsCoreSystem { get; set; } = false;

    // Colecciones del libro
    public ICollection<AttributeDefinition> Attributes { get; set; } = [];
    public ICollection<SkillDefinition> Skills { get; set; } = [];
    public ICollection<DamageTypeDefinition> DamageTypes { get; set; } = [];
    public ICollection<ConditionDefinition> Conditions { get; set; } = [];
    public ICollection<LanguageDefinition> Languages { get; set; } = [];
}