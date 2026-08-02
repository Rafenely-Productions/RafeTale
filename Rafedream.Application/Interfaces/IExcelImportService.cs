using Rafedream.Application.Services.Importer.Initializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Application.Interfaces
{
    public interface IExcelImportService
    {
        Task<(int Count, string Version)> ImportDataFromExcelAsync(Stream excelStream);
    }
}
