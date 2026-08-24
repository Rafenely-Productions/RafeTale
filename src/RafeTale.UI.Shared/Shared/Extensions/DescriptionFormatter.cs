using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RafeTale.Application.Resources;
using RafeTale.Domain.Entities;
using RafeTale.UI.Shared.Shared.Extensions.Interfaces;
using System.Text.RegularExpressions;

namespace RafeTale.UI.Shared.Shared.Extensions
{
    public partial class DescriptionFormatter(IStringLocalizer<AppStrings> factory) : IDescriptionFormatter
    {
        public MarkupString Format(string rawDescription, bool coolFormat = true)
        {
            if (string.IsNullOrEmpty(rawDescription))
                return new MarkupString(string.Empty);

            string processed = rawDescription;
            string metersLabel = factory["Label_mt"].Value;
            string feetLabel = factory["Label_ft"].Value;

            // 1. Metros
            processed = RegxMeter().Replace(processed, match =>
            {
                string meters = match.Groups[1].Value;
                return DivFormat(meters, metersLabel, coolFormat);
            });

            // 2. Pies
            processed = RegxFeet().Replace(processed, match =>
            {
                string feet = match.Groups[1].Value;
                return DivFormat(feet, feetLabel, coolFormat);
            });

            // 3. Limpieza de llaves residuales
            processed = Regx().Replace(processed, string.Empty);

            return (MarkupString)processed;
        }

        private static string DivFormat(string distance, string label, bool coolFormat = true)
        {
            if(coolFormat)
                return $"<span class=\"text-amber-400 font-mono\">{distance} {label}</span>";
            else
                return $"<span>{distance} {label}</span>";
        }

        [GeneratedRegex(@"{(?:dist|range)\s*:\s*([\d.,]+)\s*_M}", RegexOptions.IgnoreCase)]
        private static partial Regex RegxMeter();

        [GeneratedRegex(@"{(?:dist|range)\s*:\s*([\d.,]+)\s*_FT}", RegexOptions.IgnoreCase)]
        private static partial Regex RegxFeet();
        [GeneratedRegex(@"{[a-zA-Z0-9_:]+}")]
        private static partial Regex Regx();
    }
}