using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;

namespace RafeTale.Infrastructure.Extraction.Localization;

public interface ILocalizationCollector
{
    void SaveBoth(Guid entityId, LocEntity entity, LocProperty prop, string en, string localized, LocLanguage locLanguage);
    void Save(Guid entityId, LocEntity entity, LocProperty prop, string text, LocLanguage lang);
    IReadOnlyList<LocalizedContent> GetAll();
}