using RafeTale.Domain.Enums;
using RafeTale.Domain.Modifiers;
using RafeTale.Infrastructure.Extraction.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Extraction.Parsing
{
    public sealed class ModifierParser : IModifierParser
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public List<ModifierData> ParseModifiers(string json)
        {
            try
            {
                return string.IsNullOrWhiteSpace(json)
                    ? []
                    : JsonSerializer.Deserialize<List<ModifierData>>(json, _jsonOptions) ?? [];
            }
            catch (JsonException)
            {
                Debug.WriteLine($"Error de JSON en modificadores: {json}");
                return [];
            }
        }

        public List<FeatPrerequisiteModifierData> ParsePrerequisites(string json)
        {
            try
            {
                return string.IsNullOrWhiteSpace(json)
                    ? []
                    : JsonSerializer.Deserialize<List<FeatPrerequisiteModifierData>>(json, _jsonOptions) ?? [];
            }
            catch (JsonException)
            {
                Debug.WriteLine($"Error de JSON en prerequisitos: {json}");
                return [];
            }
        }

        public List<ClassTrait> ParseClassTraits(string raw)
        {
            var traits = new List<ClassTrait>();
            if (string.IsNullOrWhiteSpace(raw)) return traits;

            var pairs = raw.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(':', 2);
                if (keyValue.Length != 2) continue;

                var keyStr = keyValue[0].Trim();
                var valueStr = keyValue[1].Trim();

                var trait = new ClassTrait
                {
                    Type = Enum.Parse<ResourceType>(keyStr, true) // assuming EnumParser handles this, or use Enum.TryParse
                };

                if (keyStr.Equals("SpellSlots", StringComparison.OrdinalIgnoreCase))
                {
                    trait.SpellSlots = JsonSerializer.Deserialize<int[]>(valueStr) ?? new int[9];
                    trait.Value = string.Empty;
                }
                else
                {
                    trait.Value = valueStr;
                    trait.SpellSlots = new int[9];
                }

                traits.Add(trait);
            }
            return traits;
        }
    }
}
