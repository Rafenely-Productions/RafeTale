using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using System;

namespace RafeTale.Domain.Entities;

public class Spell : IEntity
{
    public Guid Id { get; set; }
    public string TechnicalName { get; set; } = string.Empty;
    public SpellLevel Level { get; set; } // 0 para Trucos (Cantrips), 1-9 para Conjuros
    public SchoolOfMagicEnum School { get; set; } // Evocación, Abjuración, etc.
    public CastingTime CastingTime { get; set; } // 1 Acción, 1 Reacción
    public SpellRange Range { get; set; } // Toque, 120 pies, Personal
    public string RangeDistance{ get; set; } = string.Empty;
    public List<SpellComponent> Components = []; // V, S, M (Materiales)
    public List<SpellDuration> Duration { get; set; } = null!; // En turnos, minutos, horas, etc.
    public SpellConcentration Concentration { get; set; } // "Sí" o "No"
    public bool Ritual { get; set; } // Indica si el hechizo se puede lanzar como ritual
    public List<string> ClassesTechnicalNames { get; set; } = [];
}