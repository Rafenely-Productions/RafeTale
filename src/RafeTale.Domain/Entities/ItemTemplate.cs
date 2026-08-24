using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;

namespace RafeTale.Domain.Entities;

public class ItemTemplate : IEntity
{
    public Guid Id { get; set; }
    public string TechnicalName { get; set; } = string.Empty;
    public ItemCategory Category { get; set; }
    public double Weight { get; set; }
    public int GoldValue { get; set; }

    // Para armas y armaduras
    public string? DamageDice { get; set; } // Ej: "1d8"
    public int? ArmorClass { get; set; }     // Ej: 15

    // Modificadores JSON (Igual que en Feats)
    public List<ModifierData> Modifiers { get; set; } = [];
}