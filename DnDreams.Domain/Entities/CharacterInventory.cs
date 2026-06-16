using DnDreams.Domain.Interfaces;

namespace DnDreams.Domain.Entities;

public class CharacterInventory : IEntity
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Guid ItemTemplateId { get; set; }
    public ItemTemplate Item { get; set; } = null!;

    public int Quantity { get; set; } = 1;
    public bool IsEquipped { get; set; }
    public string CustomName { get; set; } = string.Empty; // Por si renombras tu espada
}