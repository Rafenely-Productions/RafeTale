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
    public string ModifiersJson { get; set; } = "[]";

    // Propiedad calculada des-serializada en caliente para usar en el juego
    public List<ModifierData> Modifiers
    {
        get
        {
            try
            {
                return JsonSerializer.Deserialize<List<ModifierData>>(ModifiersJson) ?? new List<ModifierData>();
            }
            catch
            {
                return new List<ModifierData>();
            }
        }
    }
}

