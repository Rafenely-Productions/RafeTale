using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class TraitExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int RequiredLevel = 2;
        public const int RaceName = 3;
        public const int SubraceName = 4;
        public const int NameLoc = 5;
        public const int DescriptionLoc = 6;
        public const int DescriptionEn = 7;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Traits", isRequired: true))
        {
            var trait = new Trait
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString(),
                RequiredLevel = row.Cell(Col.RequiredLevel).TryGetValue<int>(out var res) ? res : 0,
            };

            var raceName = row.Cell(Col.RaceName).GetString();
            var SubraceName = row.Cell(Col.SubraceName).GetString();

            if (!string.IsNullOrEmpty(raceName))
            {
                trait.Race = context.Package.Races
                    .FirstOrDefault(r => r.TechnicalName.Equals(raceName, StringComparison.OrdinalIgnoreCase))!;
            }
            else if (!string.IsNullOrEmpty(SubraceName))
            {
                trait.Subrace = context.Package.Subraces
                    .FirstOrDefault(r => r.TechnicalName.Equals(SubraceName, StringComparison.OrdinalIgnoreCase))!;
            }

            context.Package.Traits.Add(trait);

            context.Localization.Save(trait.Id, LocEntity.Trait, LocProperty.Name, trait.TechnicalName, LocLanguage.en);
            context.Localization.Save(trait.Id, LocEntity.Trait, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(trait.Id, LocEntity.Trait, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(trait.Id, LocEntity.Trait, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}