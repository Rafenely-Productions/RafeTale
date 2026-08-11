using ClosedXML.Excel;
using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;

namespace RafeTale.Application.Interfaces
{
    public interface IDataExtractor
    {
        ImportDataPackage ExtractAllAsync(Stream excelStream);
    }
}
