using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Localization;

namespace RafeTale.Infrastructure.Extraction;

public class ExtractionContext
{
    public ImportDataPackage Package { get; } = new();
    public ILocalizationCollector Localization { get; }
    public LocLanguage CurrentCulture { get; }

    public ExtractionContext(LocLanguage currentCulture)
    {
        CurrentCulture = currentCulture;
        Localization = new LocalizationCollector(currentCulture);
    }
}