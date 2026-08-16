using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class SpecialTraitExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TraitName = 1;
        public const int TechnicalName = 2;
        public const int DescriptionEn = 3;
        public const int ModifiersRaw = 4;
        public const int NameLoc = 5;
        public const int DescriptionLoc = 6;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Special Traits", isRequired: true))
        {
            var traitName = row.Cell(Col.TraitName).GetString();
            var trait = context.Package.Traits
                .FirstOrDefault(r => r.TechnicalName.Equals(traitName, StringComparison.OrdinalIgnoreCase));

            var specialTrait = new SpecialTrait
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString(),
                TraitId = trait?.Id ?? Guid.Empty,
                Modifiers = ExcelParsers.ParseModifiers(row.Cell(Col.ModifiersRaw).GetString())
            };

            context.Package.SpecialTraits.Add(specialTrait);

            context.Localization.Save(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Name,
                specialTrait.TechnicalName, LocLanguage.en);
            context.Localization.Save(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}