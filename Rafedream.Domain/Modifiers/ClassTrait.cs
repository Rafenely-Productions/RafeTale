using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // 🚨 No olvides este using
using Rafedream.Domain.Enums;

namespace Rafedream.Domain.Modifiers;

public class ClassTrait
{
    // 🚨 AHORA SÍ SON PROPIEDADES REALES CON GET Y SET:
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ResourceType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public int[] SpellSlots { get; set; } = new int[9];
}