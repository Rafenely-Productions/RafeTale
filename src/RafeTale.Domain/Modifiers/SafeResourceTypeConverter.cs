using System.Text.Json;
using System.Text.Json.Serialization;
using RafeTale.Domain.Enums;

namespace RafeTale.Domain.Modifiers;

public class SafeResourceTypeConverter : JsonConverter<ResourceType>
{
    public override ResourceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (!string.IsNullOrWhiteSpace(stringValue) &&
                Enum.TryParse<ResourceType>(stringValue, ignoreCase: true, out var result))
            {
                return result;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var intValue))
        {
            if (Enum.IsDefined(typeof(ResourceType), intValue))
            {
                return (ResourceType)intValue;
            }
        }

        // Si el valor no coincide con el enum o viene nulo/corrupto, asigna Unknown en vez de romper la app
        return ResourceType.Unknown;
    }

    public override void Write(Utf8JsonWriter writer, ResourceType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}