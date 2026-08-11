using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class XpRuleExtractor : ISheetExtractor
{
    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("ReglasXP", isRequired: true))
        {
            var bonusCell = row.Cell(3);
            context.Package.XpRules.Add(new XpRules
            {
                Level = row.Cell(1).GetValue<int>(),
                RequiredXp = row.Cell(2).GetValue<int>(),
                Bonus = bonusCell.IsEmpty() ? 0 : bonusCell.GetValue<int>()
            });
        }
    }
}