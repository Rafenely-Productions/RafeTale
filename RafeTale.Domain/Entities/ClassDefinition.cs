using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;

namespace RafeTale.Domain.Entities;

public class ClassDefinition : IEntity
{
    public Guid Id { get; set; }
    public string TechnicalName { get; set; } = string.Empty; // Ej: Guerrero, Mago
    public string HitDie { get; set; } = "d8";      // Dado de golpe
    public int HitDieValue => CalculateValue();
    public ICollection<ASI> PrimaryAbility { get; set; } = new List<ASI>(); // Ej: Fuerza, Inteligencia
    public ICollection<ASI> SavingThrowProficiencies { get; set; } = new List<ASI>();// Ej: "Fuerza, Constituci�n"
    public ICollection<ArmorProficiency> ArmorProficiencies { get; set; } = new List<ArmorProficiency>();// Ej: "Armadura ligera, Armadura media"
    public ICollection<WeaponProficiency> WeaponProficiencies { get; set; } = new List<WeaponProficiency>();// Ej: "Armas simples, Armas marciales"
    public ICollection<ToolProficiency> ToolProficiencies { get; set; } = new List<ToolProficiency>(); // Ej: "Herramientas de ladr�n, Instrumentos musicales"
    public List<Skill> SkillProficiencies { get; set; } = new List<Skill>(); // Ej: "Atletismo, Percepci�n"
    public int SkillsToChoose { get; set; } = 2; // Bonificaci�n de competencia inicial
    public ICollection<ClassTrait> Feats { get; set; } = new List<ClassTrait>();

    private int CalculateValue()
    {
        if (string.IsNullOrWhiteSpace(HitDie))
            return 0;

        string[] parts = HitDie.Split('d', 'D');

        // Validar que haya al menos dos partes y que la segunda sea un entero válido
        if (parts.Length >= 2 && int.TryParse(parts[1], out int value))
        {
            return value;
        }

        return 0; // O lanzar una ArgumentException según las reglas de tu negocio
    }

    public ICollection<ClassLevelProgression> Progressions { get; set; } = new List<ClassLevelProgression>();
    public ICollection<Subclass> Subclasses { get; set; } = new List<Subclass>();
}