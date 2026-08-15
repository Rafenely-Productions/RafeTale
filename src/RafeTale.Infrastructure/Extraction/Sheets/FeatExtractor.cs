using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class FeatExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int PrerequisiteRaw = 2;
        public const int ModifiersRaw = 3;
        public const int Category = 4;
        public const int DescriptionEn = 5;
        public const int NameLoc = 6;
        public const int DescriptionLoc = 7;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Feats", isRequired: true))
        {
            var prereqRaw = row.Cell(Col.PrerequisiteRaw).GetString();
            var modsRaw = row.Cell(Col.ModifiersRaw).GetString();

            var feat = new Feat
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString() ?? string.Empty,
                Prerequisite = prereqRaw == "none" ? null : ExcelParsers.ParsePrerequisites(prereqRaw),
                Modifiers = modsRaw == "none" ? null : ExcelParsers.ParseModifiers(modsRaw),
                Category = row.Cell(Col.Category).GetEnum<CategoryFeat>()
            };

            context.Package.Feats.Add(feat);

            context.Localization.Save(feat.Id, LocEntity.Feat, LocProperty.Name,
                feat.TechnicalName, LocLanguage.en);
            context.Localization.Save(feat.Id, LocEntity.Feat, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(feat.Id, LocEntity.Feat, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(feat.Id, LocEntity.Feat, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}