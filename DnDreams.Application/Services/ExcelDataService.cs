using ClosedXML.Excel;
using DnDreams.Domain.Entities;
using DnDreams.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services
{
    public class ExcelDataService
    {
        public ExcelImportResult<Character> ImportCharactersFromExcel(Stream excelStream)
        {
            var result = new ExcelImportResult<Character>();

            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1); // Tomamos la primera pestaña

            // Leemos metadatos de celdas específicas si quieres manejar versiones
            result.GameSystem = worksheet.Cell("A1").GetString();
            result.Version = worksheet.Cell("B1").GetString();

            var rows = worksheet.RangeUsed().RowsUsed().Skip(2); // Saltamos encabezados

            foreach (var row in rows)
            {
                var character = new Character
                {
                    // Mapeo flexible: Columna 1 es Nombre, Columna 2 es Clase, etc.
                    Name = row.Cell(1).GetString(),
                    //Class = row.Cell(2).GetString(),
                    //Level = row.Cell(3).GetValue<int>(),
                    //ExperiencePoints = row.Cell(4).GetValue<int>()
                };

                result.Data.Add(character);
            }

            return result;
        }
    }
}
