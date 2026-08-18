using RafeTale.Application.Interfaces;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Application.Services
{
    public class LocalizationService(IUnitOfWork unitOfWork) : ILocalizationService
    {
        private readonly LocLanguage _currentCulture = LocLanguage.es;

        public async Task<string> GetStringAsync(Guid entityId, LocProperty property)
        {
            var entry = await unitOfWork.LocalizedContents.GetTranslationAsync(entityId, property, _currentCulture);

            // Fallback: Si no hay en español, intenta buscar en inglés ("en")
            if (entry == null && _currentCulture != LocLanguage.en)
            {
                entry = await unitOfWork.LocalizedContents.GetTranslationAsync(entityId, property, LocLanguage.en);
            }

            return entry?.Text ?? $"[{property}_Missing]";
        }

        public async Task<Dictionary<Guid, string>> GetAllAsync(LocEntity entityType, LocProperty property = LocProperty.Name)
        {
            var translations = await unitOfWork.LocalizedContents.GetManyAsync(x =>
                x.EntityType == entityType &&
                x.Property == property &&
                x.LanguageCode == _currentCulture);

            return translations.ToDictionary(t => t!.EntityId, t => t!.Text);
        }

        public async Task<List<LocalizedContent>> GetTranslationsForLanguageAsync(LocEntity entityType)
        {
            var translations = await unitOfWork.LocalizedContents.GetManyAsync(x =>
                x.EntityType == entityType &&
                x.LanguageCode == _currentCulture);
            return translations == null
                ? throw new Exception($"No translations found for entity type {entityType} and language {_currentCulture}")
                : [.. translations.OfType<LocalizedContent>()];
        }

        public async Task<Dictionary<LocProperty,Dictionary<Guid, string>>> GetAllAsync(LocEntity entityType, LocProperty[] propertyType)
        {
            try
            {
                Dictionary<LocProperty, Dictionary<Guid, string>> result = [];
                
                foreach (var property in propertyType)
                {
                    var translations = await unitOfWork.LocalizedContents.GetManyAsync(x =>
                        x.EntityType == entityType &&
                        x.Property == property &&
                        x.LanguageCode == _currentCulture);

                    result.Add(property, translations.ToDictionary(t => t!.EntityId, t => t!.Text));
                }

                return result;
            }
            catch (Exception ex)
            {
                // Manejo de errores, por ejemplo, loguear el error
                Console.WriteLine($"Error al obtener traducciones: {ex.Message}");
                return [];
            }

        }
    }
}
