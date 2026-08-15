using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class SubRaceExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int RaceName = 1;
        public const int TechnicalName = 2;
        public const int NameLoc = 3;
        public const int DescriptionLoc = 5;
        public const int DescriptionEn = 6;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Sub Races", isRequired: true))
        {
            var sub = new SubRace
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString()
            };

            var race = context.Package.Races
                .FirstOrDefault(r => r.TechnicalName.Equals(row.Cell(Col.RaceName).GetString(), StringComparison.OrdinalIgnoreCase));

            if (race != null)
                sub.RaceId = race.Id;

            context.Package.SubRaces.Add(sub);

            context.Localization.Save(sub.Id, LocEntity.SubRace, LocProperty.Name, sub.TechnicalName, LocLanguage.en);
            context.Localization.Save(sub.Id, LocEntity.SubRace, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(sub.Id, LocEntity.SubRace, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(sub.Id, LocEntity.SubRace, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}