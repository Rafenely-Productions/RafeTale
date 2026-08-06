using RafeTale.Domain.Modifiers;
using System;
using System.Collections.Generic;

namespace RafeTale.Application.DTOs
{
    public class TraitDto
    {
        public Guid Id { get; init; }
        public string TechnicalName { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty; // Para guardar la traducción localizable
        public string Description { get; init; } = string.Empty; // Para guardar la descripción HTML localizable
        public int RequiredLevel { get; init; } = 1;
        public List<ModifierData> Modifiers { get; init; } = new();
        public Guid? RaceId { get; init; }
        public Guid? SubraceId { get; init; }
    }
}