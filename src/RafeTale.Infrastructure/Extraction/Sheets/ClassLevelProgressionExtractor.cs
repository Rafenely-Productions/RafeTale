using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class ClassLevelProgressionExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int ClassName = 1;
        public const int Level = 2;
        public const int ProficiencyBonus = 3;
        public const int FeatureTechName = 4;
        public const int ClassTraitsRaw = 5;
        public const int ModifiersRaw = 6;
        public const int NameLoc = 7;
        public const int DescriptionLoc = 8;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("ClassLevelProgression", isRequired: true))
        {
            var className = row.Cell(Col.ClassName).GetString().Trim();
            var level = row.Cell(Col.Level).GetValue<int>();
            var featureTechName = row.Cell(Col.FeatureTechName).GetString().Trim();
            if (string.IsNullOrEmpty(featureTechName)) continue;

            var targetClass = context.Package.ClassDefinitions
                .FirstOrDefault(c => c.TechnicalName.Equals(className, StringComparison.OrdinalIgnoreCase));
            if (targetClass == null) continue;

            var feature = new Feature
            {
                Id = Guid.NewGuid(),
                TechnicalName = featureTechName,
                Modifiers = ExcelParsers.ParseModifiers(row.Cell(Col.ModifiersRaw).GetString()),
            };

            context.Localization.Save(feature.Id, LocEntity.Feature, LocProperty.Name, featureTechName, LocLanguage.en);
            context.Localization.Save(feature.Id, LocEntity.Feature, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(feature.Id, LocEntity.Feature, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);

            var classTraits = ExcelParsers.ParseClassTraits(row.Cell(Col.ClassTraitsRaw).GetString());

            var existing = context.Package.ClassLevelProgressions
                .FirstOrDefault(p => p.ClassDefId == targetClass.Id && p.Level == level);

            if (existing != null)
            {
                existing.Features.Add(feature);
                if (classTraits.Count != 0)
                    existing.Traits.AddRange(classTraits); // <-- BUG FIXED
            }
            else
            {
                var progression = new ClassLevelProgression
                {
                    Id = Guid.NewGuid(),
                    ClassDefId = targetClass.Id,
                    Level = level,
                    Features = [feature],
                    Traits = classTraits
                };
                context.Package.ClassLevelProgressions.Add(progression);
            }
        }
    }
}