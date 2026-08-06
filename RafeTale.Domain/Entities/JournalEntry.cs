using RafeTale.Domain.Interfaces;
using System;

namespace RafeTale.Domain.Entities;

public class JournalEntry : IEntity
{
    public Guid Id { get; set; }

    // Puede pertenecer a una Campaña en general, o ser la bitácora privada de un Personaje
    public Guid? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }

    public Guid? CharacterId { get; set; }
    public Character? Character { get; set; }

    // Datos de la nota
    public string Title { get; set; } = string.Empty; // Ej: "El misterio de la Taberna del Buey"
    public string Content { get; set; } = string.Empty; // Texto libre con los apuntes de la sesión

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int SessionNumber { get; set; } // Opcional, para ordenar cronológicamente: "Sesión 1, 2, etc."
}