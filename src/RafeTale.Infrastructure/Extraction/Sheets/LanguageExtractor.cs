using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class LanguageExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int DescriptionEn = 2;
        public const int NameLoc = 3;
        public const int DescriptionLoc = 4;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Languages",true))
        {
            var language = new Language
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString()
            };
            context.Package.Languages.Add(language);

            var loc = context.Localization;
            loc.Save(language.Id, LocEntity.Language, LocProperty.Name, language.TechnicalName, LocLanguage.en);
            loc.Save(language.Id, LocEntity.Language, LocProperty.Description, row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            loc.Save(language.Id, LocEntity.Language, LocProperty.Name, row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            loc.Save(language.Id, LocEntity.Language, LocProperty.Description, row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}