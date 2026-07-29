using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using DnDreams.Domain.Modifiers;

namespace DnDreams.Domain.Entities;

public class ClassDefinition : IEntity
{
    public Guid Id { get; set; }
    public string TechnicalName { get; set; } = string.Empty; // Ej: Guerrero, Mago
    public string HitDie { get; set; } = "d8";      // Dado de golpe
    public int HitDieValue => CalculateValue();
    public ICollection<ASI> PrimaryAbility { get; set; } = new List<ASI>(); // Ej: Fuerza, Inteligencia
    public ICollection<ASI> SavingThrowProficiencies { get; set; } = new List<ASI>();// Ej: "Fuerza, Constitución"
    public ICollection<ArmorProficiency> ArmorProficiencies { get; set; } = new List<ArmorProficiency>();// Ej: "Armadura ligera, Armadura media"
    public ICollection<WeaponProficiency> WeaponProficiencies { get; set; } = new List<WeaponProficiency>();// Ej: "Armas simples, Armas marciales"
    public ICollection<ToolProficiency> ToolProficiencies { get; set; } = new List<ToolProficiency>(); // Ej: "Herramientas de ladrón, Instrumentos musicales"
    public List<Skill> SkillProficiencies { get; set; } = new List<Skill>(); // Ej: "Atletismo, Percepción"
    public int SkillsToChoose { get; set; } = 2; // Bonificación de competencia inicial
    public ICollection<ClassTrait> Feats { get; set; } = new List<ClassTrait>();

    private int CalculateValue()
    {
        string die = HitDie.Split('d')[1]; // Quitar la "d"

        return int.Parse(die);
    }
    public ICollection<ClassLevelProgression> Progressions { get; set; } = new List<ClassLevelProgression>();
    public ICollection<Subclass> Subclasses { get; set; } = new List<Subclass>();
}