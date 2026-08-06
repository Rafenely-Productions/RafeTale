namespace RafeTale.Application.Models;


public class LevelUpRequirement
{
    public string FeatureName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RequirementType Type { get; set; }
    public List<string> Options { get; set; } = new(); // Ej: Lista de hechizos o subclases
    public bool IsCompleted { get; set; }
}

