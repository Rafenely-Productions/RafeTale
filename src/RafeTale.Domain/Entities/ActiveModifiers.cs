using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using System;

namespace RafeTale.Domain.Entities;

public class ActiveModifiers : IEntity
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    public string Source { get; set; } = string.Empty; // Ej: "Hechizo: Escudo de Fe", "Poción de Fuerza"

    // Qué propiedad va a alterar. 
    // Tip: Usa constantes o un enum para mapear fácil ("STR", "DEX", "AC", "SPEED", "SAVING_THROW_INT")
    public TargetPropertyType TargetProperty { get; set; }

    public int Value { get; set; } // Puede ser positivo (+2) o negativo (-3)

    public ModifierDurationType DurationType { get; set; }
    public int RemainingRounds { get; set; } // Solo se evalúa si DurationType == ModifierDurationType.Rounds

    // Propiedad calculada para saber si el modificador ya caducó
    public bool IsExpired => DurationType == ModifierDurationType.Rounds && RemainingRounds <= 0;
}