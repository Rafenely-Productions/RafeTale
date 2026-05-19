namespace DnDreams.Domain.Entities; 

public class ClassDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: Guerrero, Mago
    public string HitDie { get; set; } = "d8";      // Dado de golpe
    public int HitDieValue => CalculateValue();
    public string Description { get; set; } = "";

    private int CalculateValue()
    {
        string die = HitDie.Remove(HitDie.Length - 1); // Quitar la "d"

        return int.Parse(die);
    }
    public virtual ICollection<ClassLevelProgression> Progressions { get; set; } = new List<ClassLevelProgression>();
}