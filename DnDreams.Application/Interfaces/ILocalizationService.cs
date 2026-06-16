using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Interfaces
{
    public interface ILocalizationService
    {
        Task<string> GetStringAsync(Guid entityId, LocProperty property);
        Task<Dictionary<Guid, string>> GetAllAsync(LocEntity entityType, LocProperty propertyType = LocProperty.Name);
        Task<Dictionary<LocProperty,Dictionary<Guid, string>>> GetAllAsync(LocEntity entityType, LocProperty[] propertyType);
        Task<List<LocalizedContent>> GetTranslationsForLanguageAsync(LocEntity entityType);
    }
}
