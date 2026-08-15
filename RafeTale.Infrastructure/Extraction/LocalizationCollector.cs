using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;

namespace RafeTale.Infrastructure.Extraction.Localization;

public class LocalizationCollector : ILocalizationCollector
{
    private readonly Dictionary<string, LocalizedContent> _cache = new();
    private readonly LocLanguage _currentCulture;

    public LocalizationCollector(LocLanguage currentCulture)
    {
        _currentCulture = currentCulture;
    }

    public void SaveBoth(Guid entityId, LocEntity entity, LocProperty prop, string en, string localized, LocLanguage locLanguage)
    {
        Save(entityId, entity, prop, en, LocLanguage.en);
        Save(entityId, entity, prop, localized, locLanguage);
    }

    public void Save(Guid entityId, LocEntity entity, LocProperty prop, string text, LocLanguage lang)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        var key = $"{entityId}_{prop}_{lang}";
        if (_cache.TryGetValue(key, out var existing))
            existing.Text = text;
        else
            _cache.Add(key, new LocalizedContent { Id = Guid.NewGuid(), EntityId = entityId, EntityType = entity, Property = prop, Text = text, LanguageCode = lang });
    }

    public IReadOnlyList<LocalizedContent> GetAll() => _cache.Values.ToList();
}