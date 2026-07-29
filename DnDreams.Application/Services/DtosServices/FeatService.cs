using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Helpers;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DnDreams.Application.Services.DtosServices
{
    public class FeatService(IUnitOfWork uow, ILocalizationService loc) : IService<FeatDto, Feat>
    {
        // 1. ArmDto Asíncrono para consultas individuales (GetByIdAsync)
        public async Task<FeatDto> ArmDto(Feat feat)
        {
            return new FeatDto
            {
                Id = feat.Id,
                Name = await loc.GetStringAsync(feat.Id, LocProperty.Name),
                Description = await loc.GetStringAsync(feat.Id, LocProperty.Description),
                Category = feat.Category,
                Prerequisite = feat.Prerequisite,
                Modifiers = feat.Modifiers ?? new()
            };
        }

        // 2. ArmDto Síncrono rápido (Auxiliar de optimización para búsquedas en Bulk)
        public FeatDto ArmDto(Feat feat, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords)
        {
            return new FeatDto
            {
                Id = feat.Id,
                Name = localizedWords != null && localizedWords.TryGetValue(LocProperty.Name, out var nameDict) && nameDict.TryGetValue(feat.Id, out var n) ? n : feat.TechnicalName,
                Description = localizedWords != null && localizedWords.TryGetValue(LocProperty.Description, out var descDict) && descDict.TryGetValue(feat.Id, out var d) ? d : "No description available",
                Modifiers = feat.Modifiers ?? new(),
                Category = feat.Category,
                Prerequisite = feat.Prerequisite,
            };
        }

        // 3. GetAllAsync Masivo en Bloque (Usa diccionarios indexados en RAM para velocidad extrema)
        public async Task<List<FeatDto>> GetAllAsync(Expression<Func<Feat, bool>>? filter, Action<IncludeAggregator<Feat>>? includes)
        {
            var feats = await uow.Feats.GetAllAsync(filter, includes);

            // Cargamos de un solo golpe todas las traducciones de dotes
            var localizedWords = await loc.GetAllAsync(LocEntity.Feat, [LocProperty.Name, LocProperty.Description]);

            var featDtos = new List<FeatDto>();
            foreach (var feat in feats)
            {
                if (feat == null) continue;
                featDtos.Add(ArmDto(feat, localizedWords));
            }

            return featDtos.OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();
        }

        // 4. Consulta Unitaria por ID
        public async Task<FeatDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Feat>>? includes = null)
        {
            var feat = await uow.Feats.GetByIdAsync(id, includes);
            if (feat == null) return null!;

            return await ArmDto(feat);
        }
    }
}