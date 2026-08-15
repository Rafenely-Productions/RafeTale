using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;
using RafeTale.Infrastructure.Extraction.Parsing;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class BackgroundExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int ASIs = 2;
        public const int FeatName = 3;
        public const int SkillProficiencies = 4;
        public const int ToolProficienciesEn = 5;
        public const int EquipmentEn = 6;
        public const int DescriptionEn = 7;
        public const int NameLoc = 8;
        public const int ToolProficienciesLoc = 9;
        public const int EquipmentLoc = 10;
        public const int DescriptionLoc = 11;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Backgrounds", isRequired: true))
        {
            var background = new Background
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString(),
                ASIs = row.Cell(Col.ASIs).GetEnumList<AttributeImprovementChoice>(),
                SkillProficiencies = row.Cell(Col.SkillProficiencies).GetEnumList<SkillType>()
            };

            var featName = row.Cell(Col.FeatName).GetString();
            if (!string.IsNullOrEmpty(featName))
            {
                var feat = context.Package.Feats
                    .FirstOrDefault(f => f.TechnicalName.Equals(featName, StringComparison.OrdinalIgnoreCase));
                if (feat != null)
                {
                    background.FeatId = feat.Id;
                    background.Feat = feat;
                }
                else
                {
                    Console.WriteLine($"Advertencia: No se encontró el rasgo '{featName}' para el trasfondo '{background.TechnicalName}'");
                }
            }

            context.Package.Backgrounds.Add(background);

            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.Name,
                background.TechnicalName, LocLanguage.en);
            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.ToolProficiencies,
                row.Cell(Col.ToolProficienciesEn).GetString(), LocLanguage.en);
            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.Equipment,
                row.Cell(Col.EquipmentEn).GetString(), LocLanguage.en);
            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.ToolProficiencies,
                row.Cell(Col.ToolProficienciesLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.Equipment,
                row.Cell(Col.EquipmentLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(background.Id, LocEntity.Background, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}