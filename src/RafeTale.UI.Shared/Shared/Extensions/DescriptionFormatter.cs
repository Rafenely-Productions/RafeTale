using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RafeTale.Application.Resources;
using RafeTale.Domain.ValueObjects;
using RafeTale.UI.Shared.Shared.Extensions.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RafeTale.UI.Shared.Shared.Extensions
{
    public partial class DescriptionFormatter(IStringLocalizer<AppStrings> factory) : IDescriptionFormatter
    {
        public MarkupString Format(string rawDescription, bool coolFormat = true)
        {
            if (string.IsNullOrWhiteSpace(rawDescription))
                return new MarkupString(string.Empty);

            string metersLabel = factory["Label_mt"].Value;
            string feetLabel = factory["Label_ft"].Value;

            // 1. Busca e interpreta cualquier {d:valor_unidad} usando DistanceToken
            string processed = DistanceTokenRegex().Replace(rawDescription, match =>
            {
                if (!DistanceToken.TryParse(match.Value, out var token))
                    return match.Value;

                string label = token.Unit switch
                {
                    "m" => metersLabel,
                    _ => feetLabel
                };

                return DivFormat(token.Value.ToString("0.##", CultureInfo.InvariantCulture), label, coolFormat);
            });

            // 2. Limpieza de llaves residuales
            processed = CleanResidualRegex().Replace(processed, string.Empty);

            return (MarkupString)processed;
        }

        private static string DivFormat(string distance, string label, bool coolFormat = true)
        {
            return coolFormat
                ? $"<span class=\"text-amber-400 font-mono\">{distance} {label}</span>"
                : $"<span>{distance} {label}</span>";
        }

        [GeneratedRegex(@"\{d:\d+(\.\d+)?_[a-zA-Z]+\}", RegexOptions.IgnoreCase)]
        private static partial Regex DistanceTokenRegex();

        [GeneratedRegex(@"{[a-zA-Z0-9_:]+}")]
        private static partial Regex CleanResidualRegex();
    }
}