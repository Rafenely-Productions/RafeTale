using ClosedXML.Excel;
using DnDreams.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Extractors.Extensions
{
    public static class ExcelExtensions
    {
        public static IXLWorksheet GetSheet(this IXLWorkbook workbook, string sheetName, bool isRequired = true)
        {
            if (workbook.TryGetWorksheet(sheetName, out var worksheet))
            {
                return worksheet;
            }

            if (isRequired)
            {
                // Si la hoja es vital para que la App funcione, lanzamos un error descriptivo
                throw new DataImportException($"La pestaña '{sheetName}' no existe en el archivo Excel. No se puede continuar con la importación.");
            }

            return null!; // Si no es requerida, devolvemos null de forma segura
        }
    }
}
