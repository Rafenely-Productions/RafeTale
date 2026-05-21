namespace DnDreams.Domain.Entities; 

public class ClassDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: Guerrero, Mago
    public string HitDie { get; set; } = "d8";      // Dado de golpe
    public int HitDieValue => CalculateValue();
    public string Description { get; set; } = "";
    public string PrimaryAbility { get; set; } = ""; // Ej: Fuerza, Inteligencia
    public string SavingThrowProficiencies { get; set; } = ""; // Ej: "Fuerza, Constitución"
    public string ArmorProficiencies { get; set; } = ""; // Ej: "Armadura ligera, Armadura media"
    public string WeaponProficiencies { get; set; } = ""; // Ej: "Armas simples, Armas marciales"
    public string SkillProficiencies { get; set; } = ""; // Ej: "Atletismo, Percepción"
    public int SkillsToChoose { get; set; } = 2; // Bonificación de competencia inicial

    private int CalculateValue()
    {
        string die = HitDie.Remove(HitDie.Length - 1); // Quitar la "d"

        return int.Parse(die);
    }
    public virtual ICollection<ClassLevelProgression> Progressions { get; set; } = new List<ClassLevelProgression>();
}