namespace DnDreams.Domain.Entities; 

public class ClassDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: Guerrero, Mago
    public string HitDie { get; set; } = "d8";      // Dado de golpe
    public List<ClassLevelProgression> Progression { get; set; } = new();
}