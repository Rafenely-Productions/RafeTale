using DnDreams.Domain.Interfaces;
using System;

namespace DnDreams.Domain.Entities;

public class CharacterSpellSlots : IEntity
{
    public Guid Id { get; set; } // Tu clave primaria física
    public Guid CharacterId { get; set; } // Tu FK
    public virtual Character Character { get; set; } = null!; // Propiedad de navegación

    public int Level { get; set; }
    public int MaxSlots { get; set; }
    public int UsedSlots { get; set; }

    // Propiedad calculada para Blazor/MAUI
    public int RemainingSlots => Math.Max(0, MaxSlots - UsedSlots);
}