using ClosedXML.Excel;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction;
using RafeTale.Infrastructure.Extraction.Interfaces;
using RafeTale.Infrastructure.Extraction.Sheets;

namespace RafeTale.Tests.Infrastructure.Extraction;

public static class ExcelTestHelpers
{
    public static ExtractionContext CreateContext() => new(LocLanguage.es);

    public static XLWorkbook CreateWorkbook(string sheetName, string[] headers, params string[][] rows)
    {
        var wb = new XLWorkbook();
        AddSheet(wb, sheetName, headers, rows);
        return wb;
    }

    public static void AddSheet(XLWorkbook workbook, string name, string[] headers, string[][] rows)
    {
        var ws = workbook.Worksheets.Add(name);
        for (int i = 0; i < headers.Length; i++)
            ws.Cell(1, i + 1).Value = headers[i];

        for (int r = 0; r < rows.Length; r++)
            for (int c = 0; c < rows[r].Length; c++)
                ws.Cell(r + 2, c + 1).Value = rows[r][c];
    }

    public static Stream SaveToStream(XLWorkbook workbook)
    {
        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    /// <summary>Full pipeline in dependency order — mirrors MauiProgram registration.</summary>
    public static ExcelDataExtractor CreateFullPipeline() => new(new ISheetExtractor[]
    {
        new LanguageExtractor(),
        new SkillExtractor(),
        new RaceExtractor(),
        new SubRaceExtractor(),
        new TraitExtractor(),
        new SpecialTraitExtractor(),
        new ClassExtractor(),
        new SubclassExtractor(),
        new SpellExtractor(),
        new FeatExtractor(),
        new BackgroundExtractor(),
        new CharacterExtractor(),
        new ItemExtractor(),
        new ClassLevelProgressionExtractor(),
        new SubclassLevelProgressionExtractor(),
        new XpRuleExtractor(),
    });
}