using System;

namespace DnDreams.Domain.Entities;

public class Spell
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; } // 0 para Trucos (Cantrips), 1-9 para Conjuros
    public string School { get; set; } = string.Empty; // Evocación, Abjuración, etc.
    public string CastingTime { get; set; } = string.Empty; // 1 Acción, 1 Reacción
    public string Range { get; set; } = string.Empty; // Toque, 120 pies, Personal
    public string Description { get; set; } = string.Empty;
}