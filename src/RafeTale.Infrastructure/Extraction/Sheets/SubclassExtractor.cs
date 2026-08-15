using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class SubclassExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int ClassName = 1;
        public const int TechnicalName = 2;
        public const int DescriptionEn = 3;
        public const int NameLoc = 4;
        public const int DescriptionLoc = 5;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("SubClasses", isRequired: true))
        {
            var className = row.Cell(Col.ClassName).GetString().Trim();
            var techName = row.Cell(Col.TechnicalName).GetString().Trim();

            var classDef = context.Package.ClassDefinitions
                .FirstOrDefault(p => p.TechnicalName.Equals(className, StringComparison.OrdinalIgnoreCase));

            var subClass = new Subclass
            {
                Id = Guid.NewGuid(),
                ClassDefinition = classDef!,
                TechnicalName = techName,
                Progressions = []
            };

            if (classDef != null)
            {
                classDef.Subclasses ??= [];
                classDef.Subclasses.Add(subClass);
            }

            context.Package.Subclasses.Add(subClass);

            context.Localization.Save(subClass.Id, LocEntity.Subclass, LocProperty.Name,
                techName, LocLanguage.en);
            context.Localization.Save(subClass.Id, LocEntity.Subclass, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(subClass.Id, LocEntity.Subclass, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(subClass.Id, LocEntity.Subclass, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}