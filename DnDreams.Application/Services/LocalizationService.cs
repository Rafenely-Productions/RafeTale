using DnDreams.Application.Interfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
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
        private readonly LocLanguage _currentCulture;

        public LocalizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _currentCulture = LocLanguage.es;
        }

        public async Task<string> GetStringAsync(Guid entityId, LocProperty property)
        {
            var entry = await _unitOfWork.LocalizedContents.GetTranslationAsync(entityId, property, _currentCulture);

            // Fallback: Si no hay en español, intenta buscar en inglés ("en")
            if (entry == null && _currentCulture != LocLanguage.en)
            {
                entry = await _unitOfWork.LocalizedContents.GetTranslationAsync(entityId, property, LocLanguage.en);
            }

            return entry?.Text ?? $"[{property}_Missing]";
        }

        public async Task<Dictionary<Guid, string>> GetAllAsync(LocEntity entityType, LocProperty property = LocProperty.Name)
        {
            var translations = await _unitOfWork.LocalizedContents.GetManyAsync(x =>
                x.EntityType == entityType &&
                x.Property == property &&
                x.LanguageCode == _currentCulture);

            return translations.ToDictionary(t => t.EntityId, t => t.Text);
        }
        public async Task<List<LocalizedContent?>> GetTranslationsForLanguageAsync(LocEntity entityType)
        {
            var translations = await _unitOfWork.LocalizedContents.GetManyAsync(x =>
                x.EntityType == entityType &&
                x.LanguageCode == _currentCulture);

            return translations.ToList();
        }

        public async Task<Dictionary<LocProperty,Dictionary<Guid, string>>> GetAllAsync(LocEntity entityType, LocProperty[] propertyType)
        {
            try
            {
                Dictionary<LocProperty, Dictionary<Guid, string>> result = new Dictionary<LocProperty, Dictionary<Guid, string>>();
                
                foreach (var property in propertyType)
                {
                    var translations = await _unitOfWork.LocalizedContents.GetManyAsync(x =>
                        x.EntityType == entityType &&
                        x.Property == property &&
                        x.LanguageCode == _currentCulture);

                    result.Add(property, translations.ToDictionary(t => t.EntityId, t => t.Text));
                }

                return result;
            }
            catch (Exception ex)
            {
                // Manejo de errores, por ejemplo, loguear el error
                Console.WriteLine($"Error al obtener traducciones: {ex.Message}");
                return new Dictionary<LocProperty, Dictionary<Guid, string>>();
            }

        }
    }
}
