using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class RaceExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int CreatureType = 2;
        public const int Size = 3;
        public const int Speed = 4;
        public const int NameLoc = 5;
        public const int DescriptionLoc = 6;
        public const int DescriptionEn = 7;
        public const int Languages = 8;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        var allLanguages = context.Package.Languages; // already populated

        foreach (var row in workbook.GetDataRows("Races", isRequired: true))
        {
            var race = new Race
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString(),
                CreatureType = row.Cell(Col.CreatureType).GetEnum<CreatureType>(),
                Size = row.Cell(Col.Size).GetEnum<SizeCategory>(),
                Speed = row.Cell(Col.Speed).GetString(),
            };

            // Resolve relationships
            var langNames = row.Cell(Col.Languages).GetString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim());
            foreach (var ln in langNames)
            {
                var found = allLanguages.FirstOrDefault(l => l.TechnicalName.Equals(ln, StringComparison.OrdinalIgnoreCase));
                if (found != null) race.Languages.Add(found);
                else Console.WriteLine($"Advertencia: Idioma '{ln}' no encontrado para raza '{race.TechnicalName}'");
            }

            context.Package.Races.Add(race);

            context.Localization.SaveBoth(race.Id, LocEntity.Race, LocProperty.Name,
                race.TechnicalName, row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(race.Id, LocEntity.Race, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(race.Id, LocEntity.Race, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}