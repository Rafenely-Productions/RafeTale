using ClosedXML.Excel;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Modifiers;
using RafeTale.Infrastructure.Extraction.Extensions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.Json;
using System.Text.Json.Serialization;
using RafeTale.Infrastructure.Extraction.Sheets;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction
{
    public class ExcelDataExtractor : IDataExtractor
    {
        private IReadOnlyList<ISheetExtractor> _extractors = [];
        public ExcelDataExtractor(IEnumerable<ISheetExtractor> extractors)
        {
            _extractors = extractors.ToList();
        }
        private readonly LocLanguage _currentCulture;

        private Dictionary<string, LocalizedContent> _localizedContentCache = new();

        public ImportDataPackage ExtractAllAsync(Stream excelStream)
        {
            using var workbook = new XLWorkbook(excelStream);
            var context = new ExtractionContext(LocLanguage.es);

            foreach (var extractor in _extractors)
            {
                extractor.Extract(workbook, context);
            }

            context.Package.LocalizedContents.AddRange(context.Localization.GetAll());
            return context.Package;
        }
    }
}
