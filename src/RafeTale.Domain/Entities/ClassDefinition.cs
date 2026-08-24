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
    public ICollection<AttributeImprovementChoice> PrimaryAbility { get; set; } =[]; // Ej: Fuerza, Inteligencia
    public ICollection<AttributeImprovementChoice> SavingThrowProficiencies { get; set; } = [];// Ej: "Fuerza, Constituci�n"
    public ICollection<ArmorProficiency> ArmorProficiencies { get; set; } = [];// Ej: "Armadura ligera, Armadura media"
    public ICollection<WeaponProficiency> WeaponProficiencies { get; set; } = [];// Ej: "Armas simples, Armas marciales"
    public ICollection<ToolProficiency> ToolProficiencies { get; set; } = []; // Ej: "Herramientas de ladr�n, Instrumentos musicales"
    public List<Skill> SkillProficiencies { get; set; } = []; // Ej: "Atletismo, Percepci�n"
    public int SkillsToChoose { get; set; } = 2; // Bonificaci�n de competencia inicial
    public ICollection<ClassTrait> Feats { get; set; } = [];

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

    public ICollection<ClassLevelProgression> Progressions { get; set; } = [];
    public ICollection<Subclass> Subclasses { get; set; } = [];
}