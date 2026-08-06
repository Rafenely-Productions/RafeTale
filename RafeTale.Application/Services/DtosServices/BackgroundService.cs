using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Interfaces.DtosInterfaces;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Helpers;
using RafeTale.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RafeTale.Application.Services.DtosServices
{
    public class BackgroundService(IUnitOfWork uow, ILocalizationService loc) : IService<BackgroundDto, Background>
    {
        // 1. ArmDto Asíncrono para consultas individuales
        public async Task<BackgroundDto> ArmDto(Background background)
        {
            FeatDto? featDto = null;
            if (background.FeatId != Guid.Empty)
            {
                var feat = await uow.Feats.GetByIdAsync(background.FeatId);
                if (feat != null)
                {
                    featDto = new FeatDto
                    {
                        Id = feat.Id,
                        Name = await loc.GetStringAsync(feat.Id, LocProperty.Name),
                        Description = await loc.GetStringAsync(feat.Id, LocProperty.Description),
                    };
                }
            }

            return new BackgroundDto
            {
                Id = background.Id,
                TechnicalName = background.TechnicalName,
                Name = await loc.GetStringAsync(background.Id, LocProperty.Name),
                Description = await loc.GetStringAsync(background.Id, LocProperty.Description),
                ToolProficiencies = await loc.GetStringAsync(background.Id, LocProperty.ToolProficiencies),
                Equipment = await loc.GetStringAsync(background.Id, LocProperty.Equipment),
                ASIs = background.ASIs,
                SkillProficiencies = background.SkillProficiencies,
                FeatId = background.FeatId,
                Feat = featDto
            };
        }

        // 2. ArmDto Síncrono rápido (Auxiliar de GetAllAsync)
        public BackgroundDto ArmDto(Background background, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords)
        {
            return new BackgroundDto
            {
                Id = background.Id,
                TechnicalName = background.TechnicalName,
                Name = localizedWords != null && localizedWords.TryGetValue(LocProperty.Name, out var nameDict) && nameDict.TryGetValue(background.Id, out var n) ? n : background.TechnicalName,
                Description = localizedWords != null && localizedWords.TryGetValue(LocProperty.Description, out var descDict) && descDict.TryGetValue(background.Id, out var d) ? d : "[No Description]",
                ToolProficiencies = localizedWords != null && localizedWords.TryGetValue(LocProperty.ToolProficiencies, out var toolDict) && toolDict.TryGetValue(background.Id, out var t) ? t : string.Empty,
                Equipment = localizedWords != null && localizedWords.TryGetValue(LocProperty.Equipment, out var eqDict) && eqDict.TryGetValue(background.Id, out var e) ? e : string.Empty,
                ASIs = background.ASIs,
                SkillProficiencies = background.SkillProficiencies,
                FeatId = background.FeatId,
                Feat = null // Se inyectará en GetAllAsync si corresponde
            };
        }

        // 3. GetAllAsync Masivo con Diccionarios Indexados (Carga super veloz)
        public async Task<List<BackgroundDto>> GetAllAsync(Expression<Func<Background, bool>>? filter, Action<IncludeAggregator<Background>>? includes)
        {
            var backgrounds = await uow.Backgrounds.GetAllAsync(filter, includes);

            var localizedFeats = await loc.GetAllAsync(LocEntity.Feat, [LocProperty.Name, LocProperty.Description]);

            var localizedWords = await loc.GetAllAsync(LocEntity.Background,
                [LocProperty.Name, LocProperty.Description, LocProperty.ToolProficiencies, LocProperty.Equipment]);

            var backgroundDtos = new List<BackgroundDto>();
            foreach (var bg in backgrounds)
            {
                var baseDto = ArmDto(bg!, localizedWords);

                FeatDto? associatedFeat = null;
                if (bg.FeatId != Guid.Empty)
                {
                    var feat = bg.Feat;
                    if (feat != null)
                    {
                        associatedFeat = new FeatDto
                        {
                            Id = feat.Id,
                            Name = localizedFeats.TryGetValue(LocProperty.Name, out var nameDict) && nameDict.TryGetValue(feat.Id, out var n) ? n : feat.TechnicalName,
                            Description = localizedFeats.TryGetValue(LocProperty.Description, out var descDict) && descDict.TryGetValue(feat.Id, out var d) ? d : "No description available",
                        };
                    }
                }

                var completeDto = new BackgroundDto
                {
                    Id = baseDto.Id,
                    TechnicalName = baseDto.TechnicalName,
                    Name = baseDto.Name,
                    Description = baseDto.Description,
                    ToolProficiencies = baseDto.ToolProficiencies,
                    Equipment = baseDto.Equipment,
                    ASIs = baseDto.ASIs,
                    SkillProficiencies = baseDto.SkillProficiencies,
                    FeatId = baseDto.FeatId,
                    Feat = associatedFeat
                };

                backgroundDtos.Add(completeDto);
            }
            return backgroundDtos;
        }

        public async Task<BackgroundDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Background>>? includes = null)
        {
            var background = await uow.Backgrounds.GetByIdAsync(id, includes);
            if (background == null) return null!;

            return await ArmDto(background);
        }
    }
}