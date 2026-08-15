using ClosedXML.Excel;
using RafeTale.Application.Services.Importer;

namespace RafeTale.Infrastructure.Extraction.Interfaces
{
    public interface ISheetExtractor
    {
        void Extract(IXLWorkbook workbook, ExtractionContext package);
    }
}