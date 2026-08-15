using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Interfaces.DtosInterfaces;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Helpers;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RafeTale.Application.Services.DtosServices
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
                Category = feat.Category.ToString(),
                Prerequisite = ArmPrerequisite(feat.Prerequisite),
                Modifiers = ArmModifier(feat.Modifiers)
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
                Modifiers = ArmModifier(feat.Modifiers),
                Category = feat.Category.ToString(),
                Prerequisite = ArmPrerequisite(feat.Prerequisite),
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

        private List<ModifierDataDto> ArmModifier(List<ModifierData> modifier)
        {
            List<ModifierDataDto> modifiers = new List<ModifierDataDto>();
            foreach (var mod in modifier)
            {
                modifiers.Add(new ModifierDataDto
                {
                    Type = (ModifierTypeDto)mod.Type,
                    Target = mod.Target.ToString(),
                    Value = mod.Value
                });
            }
            return modifiers;
        }

        private List<FeatPrerequisiteModifierDataDto> ArmPrerequisite(List<FeatPrerequisiteModifierData> prerequisite)
        {
            List<FeatPrerequisiteModifierDataDto> prerequisites = new List<FeatPrerequisiteModifierDataDto>();
            foreach (var prereq in prerequisite)
            {
                prerequisites.Add(new FeatPrerequisiteModifierDataDto
                {
                    Type = prereq.Type.ToString(),
                    Target = prereq.Target.ToString(),
                    Value = prereq.Value
                });
            }
            return prerequisites;
        }
    }
}