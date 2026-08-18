using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction;

public class ExtractionContext(LocLanguage currentCulture)
{
    public ImportDataPackage Package { get; } = new();
    public ILocalizationCollector Localization { get; } = new LocalizationCollector(currentCulture);
    public LocLanguage CurrentCulture { get; } = currentCulture;
}