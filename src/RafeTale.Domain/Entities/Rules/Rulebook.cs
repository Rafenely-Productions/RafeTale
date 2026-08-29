using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Entities.Rules;

public class Rulebook : IEntity
{
    public Guid Id { get; set; } 
    public string BookId { get; set; } = string.Empty;   
    public string? SystemId { get; set; }                  
    public string Title { get; set; } = string.Empty;       
    public string Type { get; set; } = string.Empty;
    public string Version { get; set; } = "";
    public string Author { get; set; } = "";
    public string Notes { get; set; } = "";

    public string DefaultLanguage { get; set; } = "es";        // ej: "es", "en"
    public List<string> SupportedLanguages { get; set; } = [ "es", "en" ];
    // Colecciones del libro
    public ICollection<AttributeDefinition> Attributes { get; set; } = [];
    public ICollection<SkillDefinition> Skills { get; set; } = [];
    public ICollection<DamageTypeDefinition> DamageTypes { get; set; } = [];
    public ICollection<ConditionDefinition> Conditions { get; set; } = [];
    public ICollection<LanguageDefinition> Languages { get; set; } = [];
    public ICollection<CreatureTypeDefinition> CreatureTypes { get; set; } = [];
}