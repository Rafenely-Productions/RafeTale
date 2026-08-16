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
                    ? new List<ModifierData>()
                    : JsonSerializer.Deserialize<List<ModifierData>>(json, _jsonOptions) ?? new List<ModifierData>();
            }
            catch (JsonException)
            {
                Debug.WriteLine($"Error de JSON en modificadores: {json}");
                return new List<ModifierData>();
            }
        }

        public List<FeatPrerequisiteModifierData> ParsePrerequisites(string json)
        {
            try
            {
                return string.IsNullOrWhiteSpace(json)
                    ? new List<FeatPrerequisiteModifierData>()
                    : JsonSerializer.Deserialize<List<FeatPrerequisiteModifierData>>(json, _jsonOptions) ?? new List<FeatPrerequisiteModifierData>();
            }
            catch (JsonException)
            {
                Debug.WriteLine($"Error de JSON en prerequisitos: {json}");
                return new List<FeatPrerequisiteModifierData>();
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
                    trait.Value = null;
                }
                else
                {
                    trait.Value = valueStr;
                    trait.SpellSlots = null;
                }

                traits.Add(trait);
            }
            return traits;
        }
    }
}
