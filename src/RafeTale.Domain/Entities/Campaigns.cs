using RafeTale.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace RafeTale.Domain.Entities;

public class Campaign : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: "La Maldición de Strahd"
    public string DungeonMasterName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty; // Sinopsis o descripción de la mesa

    // Relación Muchos a Muchos con los Personajes a través de la tabla intermedia
    public List<CampaignCharacter> CampaignCharacters { get; set; } = new();
}