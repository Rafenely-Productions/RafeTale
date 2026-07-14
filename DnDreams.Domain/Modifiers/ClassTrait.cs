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

