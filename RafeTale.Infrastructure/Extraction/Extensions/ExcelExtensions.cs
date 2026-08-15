using ClosedXML.Excel;
using RafeTale.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Extraction.Extensions
{
    public static class WorkbookExtensions
    {
        public static IXLWorksheet? GetSheetSafe(this IXLWorkbook wb, string name)
       => wb.TryGetWorksheet(name, out var sheet) ? sheet : null;

        public static IEnumerable<IXLRangeRow> GetDataRows(this IXLWorkbook wb, string sheetName, bool isRequired)
        {
            var sheet = wb.TryGetWorksheet(sheetName, out var s) ? s : null;
            if (sheet == null)
            {
                if (isRequired) throw new DataImportException($"La pestaña '{sheetName}' no existe en el archivo Excel.");
                return Enumerable.Empty<IXLRangeRow>();
            }
            return sheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>();
        }
    }
}
