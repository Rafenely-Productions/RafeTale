using DnDreams.Domain.Interfaces;
using System;

namespace DnDreams.Domain.Entities;

public class CharacterSpellSlots : IEntity
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    // 🛠️ MODIFICADO: Aunque son enteros, limitamos por lógica de negocio
    private int _slotLevel;
    public int SlotLevel
    {
        get => _slotLevel;
        set => _slotLevel = (value >= 1 && value <= 9) ? value : throw new ArgumentOutOfRangeException("Los slots de conjuro van del nivel 1 al 9.");
    }

    public int TotalSlots { get; set; }
    public int UsedSlots { get; set; }

    public int AvailableSlots => Math.Max(0, TotalSlots - UsedSlots);
}