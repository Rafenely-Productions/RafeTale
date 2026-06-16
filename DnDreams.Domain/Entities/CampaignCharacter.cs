using DnDreams.Domain.Interfaces;
using System;

namespace DnDreams.Domain.Entities;

public class CampaignCharacter : IEntity
{
    public Guid Id { get; set; }

    // Relación con la Campaña
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;

    // Relación con el Personaje
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}