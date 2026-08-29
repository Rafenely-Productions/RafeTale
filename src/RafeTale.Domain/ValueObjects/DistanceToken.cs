using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RafeTale.Domain.ValueObjects;

/// <summary>
/// Representa un valor de distancia tipado extraído de un token con formato {d:valor_unidad}.
/// Diseñado para la capa de Dominio sin dependencias de UI ni texto de visualización.
/// </summary>
public readonly partial record struct DistanceToken(double Value, string Unit)
{
    // Patrón optimizado con Source Generator: captura cualquier número positivo y una unidad alfanumérica
    [GeneratedRegex(@"\{d:(?<val>\d+(\.\d+)?)_(?<unit>[a-zA-Z]+)\}", RegexOptions.IgnoreCase)]
    private static partial Regex DistanceRegex();

    /// <summary>
    /// Parsea una cadena de texto. Si falla, retorna por defecto 30 ft.
    /// </summary>
    public static DistanceToken Parse(string token)
    {
        if (TryParse(token, out var result))
            return result;

        return new DistanceToken(30, "ft");
    }

    /// <summary>
    /// Intenta parsear una cadena con formato {d:valor_unidad}.
    /// </summary>
    public static bool TryParse(string token, out DistanceToken result)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            result = default;
            return false;
        }

        var match = DistanceRegex().Match(token);
        if (!match.Success)
        {
            result = default;
            return false;
        }

        double val = double.Parse(match.Groups["val"].Value, CultureInfo.InvariantCulture);
        string unit = match.Groups["unit"].Value.ToLowerInvariant();

        // Solo admitimos los 4 códigos base del sistema
        if (unit is not ("ft" or "m" or "km" or "mi"))
        {
            result = default;
            return false;
        }

        result = new DistanceToken(val, unit);
        return true;
    }

    /// <summary>
    /// Convierte la distancia a la unidad destino deseada ('ft', 'm', 'km', 'mi').
    /// Aplica la regla estándar de equivalencia TTRPG (5 ft = 1.5 m / 30 ft = 9 m).
    /// </summary>
    public DistanceToken ConvertTo(string targetUnit)
    {
        string target = targetUnit.ToLowerInvariant();
        if (Unit == target) return this;

        double convertedValue = (Unit, target) switch
        {
            // Pies <-> Metros (Regla estándar 5e: 5 ft = 1.5 m)
            ("ft", "m") => Math.Round((Value / 5.0) * 1.5, 1),
            ("m", "ft") => Math.Round((Value / 1.5) * 5.0),

            // Millas <-> Kilómetros
            ("mi", "km") => Math.Round(Value * 1.60934, 2),
            ("km", "mi") => Math.Round(Value / 1.60934, 2),

            // Metros <-> Kilómetros
            ("m", "km") => Value / 1000.0,
            ("km", "m") => Value * 1000.0,

            // Pies <-> Millas
            ("ft", "mi") => Math.Round(Value / 5280.0, 2),
            ("mi", "ft") => Value * 5280.0,

            _ => Value // Si no hay regla de conversión directa, conserva el valor base
        };

        return new DistanceToken(convertedValue, target);
    }
}