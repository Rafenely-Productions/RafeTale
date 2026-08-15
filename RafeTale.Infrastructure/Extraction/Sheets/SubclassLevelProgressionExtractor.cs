using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class SubclassLevelProgressionExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int ClassName = 1;
        public const int SubclassName = 2;
        public const int FeatureTechName = 3;
        public const int Level = 4;
        public const int ModifiersRaw = 5;
        public const int NameLoc = 6;
        public const int DescriptionLoc = 7;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("SubClassLevelProgresion", isRequired: true))
        {
            var subclassName = row.Cell(Col.SubclassName).GetString().Trim();
            var featureTechName = row.Cell(Col.FeatureTechName).GetString().Trim();
            var level = row.Cell(Col.Level).GetValue<int>();

            var targetSubclass = context.Package.Subclasses
                .FirstOrDefault(c => c.TechnicalName.Equals(subclassName, StringComparison.OrdinalIgnoreCase));

            if (targetSubclass == null) continue;

            var feature = new Feature
            {
                Id = Guid.NewGuid(),
                TechnicalName = featureTechName,
                RequiresChoice = featureTechName.Contains("Elegir", StringComparison.OrdinalIgnoreCase) ||
                                 featureTechName.Contains("Arquetipo", StringComparison.OrdinalIgnoreCase),
            };

            context.Localization.Save(feature.Id, LocEntity.Feature, LocProperty.Name,
                featureTechName, LocLanguage.en);
            context.Localization.Save(feature.Id, LocEntity.Feature, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(feature.Id, LocEntity.Feature, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);

            var existing = context.Package.SubclassLevelProgressions
                .FirstOrDefault(p => p.SubclassId == targetSubclass.Id && p.Level == level);

            if (existing != null)
            {
                existing.Features.Add(feature);
            }
            else
            {
                var progression = new SubclassLevelProgression
                {
                    Id = Guid.NewGuid(),
                    Level = level,
                    SubclassId = targetSubclass.Id,
                    Subclass = targetSubclass,
                    Features = new List<Feature> { feature }
                };
                targetSubclass.Progressions ??= new List<SubclassLevelProgression>();
                targetSubclass.Progressions.Add(progression);
                context.Package.SubclassLevelProgressions.Add(progression);
            }
        }
    }
}