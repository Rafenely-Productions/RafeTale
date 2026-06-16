using System;
using System.Collections.Generic;
using System.Text.Json;
using DnDreams.Domain.Enums;

namespace DnDreams.Domain.Modifiers;
public class ClassTrait
{
    public ResourceType Type;
    public string Value = string.Empty;
    public int[] SpellSlots { get;  set; } = new int[9];
}

public class ProgressionSnapshot
{
    public int[] SpellSlots { get; private set; } = new int[9];
    private readonly Dictionary<ResourceType, string> _resources = new();

    // Propiedad fuertemente tipada para la matriz de espacios de conjuros
  
    public ProgressionSnapshot(string? rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData)) return;

        // Separamos las propiedades por la pleca '|'
        
    }

    private void ParseSpellSlots(string rawArray)
    {
        try
        {
            var slots = JsonSerializer.Deserialize<int[]>(rawArray);
            if (slots != null && slots.Length == 9)
            {
                SpellSlots = slots;
            }
        }
        catch
        {
            SpellSlots = new int[9];
        }
    }

    // Métodos de acceso seguros por ENUM
    public int GetValueAsInt(ResourceType resource, int defaultValue = 0)
    {
        if (_resources.TryGetValue(resource, out var val) && int.TryParse(val, out var result))
            return result;
        return defaultValue;
    }

    public string GetValueAsString(ResourceType resource, string defaultValue = "")
    {
        return _resources.TryGetValue(resource, out var val) ? val : defaultValue;
    }

    // Propiedad de conveniencia para verificar si tiene algún recurso
    public bool HasResource(ResourceType resource) => _resources.ContainsKey(resource);
}