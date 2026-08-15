using ClosedXML.Excel;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Modifiers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RafeTale.Infrastructure.Extraction.Parsing;

public static class ExcelParsers
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static T ParseEnum<T>(this string? input) where T : struct, Enum
        => Enum.TryParse<T>(input?.Trim(), true, out var r) ? r : default;

    public static T GetEnum<T>(this IXLCell cell) where T : struct, Enum
        => cell.GetString().ParseEnum<T>();

    public static List<T> GetEnumList<T>(this IXLCell cell) where T : struct, Enum
    {
        var v = cell.GetString();
        if (string.IsNullOrWhiteSpace(v)) return new List<T>();
        return v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => Enum.TryParse<T>(s, true, out _))
                .Select(s => Enum.Parse<T>(s, true))
                .ToList();
    }

    public static List<ModifierData> ParseModifiers(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<ModifierData>();
        try { return JsonSerializer.Deserialize<List<ModifierData>>(json, JsonOptions) ?? new(); }
        catch (JsonException) { Console.WriteLine($"JSON inválido (modifiers): {json}"); return new(); }
    }

    public static List<FeatPrerequisiteModifierData> ParsePrerequisites(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<FeatPrerequisiteModifierData>();
        try { return JsonSerializer.Deserialize<List<FeatPrerequisiteModifierData>>(json, JsonOptions) ?? new(); }
        catch (JsonException) { Console.WriteLine($"JSON inválido (prerequisites): {json}"); return new(); }
    }

    public static List<ClassTrait> ParseClassTraits(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new List<ClassTrait>();
        var traits = new List<ClassTrait>();
        foreach (var pair in raw.Split('|', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split(':', 2);
            if (kv.Length != 2) continue;
            var key = kv[0].Trim();
            var val = kv[1].Trim();

            var trait = new ClassTrait { Type = key.ParseEnum<ResourceType>() };
            if (key.Equals("SpellSlots", StringComparison.OrdinalIgnoreCase))
            {
                trait.SpellSlots = JsonSerializer.Deserialize<int[]>(val) ?? new int[9];
                trait.Value = null;
            }
            else
            {
                trait.Value = val;
                trait.SpellSlots = null;
            }
            traits.Add(trait);
        }
        return traits;
    }
}