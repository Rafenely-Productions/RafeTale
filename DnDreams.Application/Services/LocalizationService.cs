using DnDreams.Application.Interfaces;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _currentCulture;

        public LocalizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _currentCulture = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        }

        public async Task<string> GetStringAsync(Guid entityId, string property)
        {
            var entry = await _unitOfWork.LocalizedContents.GetTranslationAsync(entityId, property, _currentCulture);

            // Fallback: Si no hay en español, intenta buscar en inglés ("en")
            if (entry == null && _currentCulture != "en")
            {
                entry = await _unitOfWork.LocalizedContents.GetTranslationAsync(entityId, property, "en");
            }

            return entry?.Text ?? $"[{property}_Missing]";
        }

        public async Task<Dictionary<Guid, string>> GetAllAsync(string entityType,string property = "Name")
        {
            var translations = await _unitOfWork.LocalizedContents.GetManyAsync(x =>
                x.EntityType == entityType &&
                x.Property == property &&
                x.LanguageCode == _currentCulture);

            return translations.ToDictionary(t => t.EntityId, t => t.Text);
        }
    }
}
