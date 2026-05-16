using System;
using DnDreams.Domain.Enums;
namespace DnDreams.Domain.Entities;



public class CharacterModifier
{
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty; // Ej: "Dote: Actor", "ASI Nivel 4"
    public ModifierType Type { get; set; }
    public string Target { get; set; } = "";// Ej: "Charisma", "Spell_Shield", "MaxHp"
    public int Value { get; set; } // El número que suma o afecta (1, 2, -5, etc.)

    public Guid CharacterId { get; set; }
}