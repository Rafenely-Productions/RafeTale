using System;
using System.Text.Json;
using DnDreams.Domain.Modifiers;

namespace DnDreams.Domain.Entities;

public class Feat
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Prerequisite { get; set; } = "Ninguno"; // Ej: "Fuerza 13 o más" o "Capacidad de lanzar conjuros"
    public List<ModifierData> Modifiers { get; set; } = new();

}

