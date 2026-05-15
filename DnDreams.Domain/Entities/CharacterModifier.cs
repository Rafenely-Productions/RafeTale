using System;

namespace DnDreams.Domain.Entities;

public enum ModifierType
{
    AttributeBonus,  // Ej: +1 a Carisma, +2 a Fuerza
    GrantSpell,      // Ej: Te da el hechizo "Escudo"
    GrantFeature,    // Ej: Te da la habilidad "Visión en la Oscuridad"
    HpBonus          // Ej: El dote Robustez da +2 HP por nivel
}

public class CharacterModifier
{
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty; // Ej: "Dote: Actor", "ASI Nivel 4"
    public ModifierType Type { get; set; }
    public string Target { get; set; } = string.Empty; // Ej: "Charisma", "Spell_Shield", "MaxHp"
    public int Value { get; set; } // El número que suma o afecta (1, 2, -5, etc.)

    public Guid CharacterId { get; set; }
}