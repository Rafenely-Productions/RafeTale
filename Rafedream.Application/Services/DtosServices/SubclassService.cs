using Rafedream.Application.DTOs;
using Rafedream.Domain.Helpers;
using Rafedream.Application.Interfaces;
using Rafedream.Application.Interfaces.DtosInterfaces;
using Rafedream.Domain.Entities;
using Rafedream.Domain.Enums;
using Rafedream.Domain.Interfaces;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Linq.Expressions;

namespace Rafedream.Application.Services.DtosServices
{
    public class SubclassService : IService<SubclassDto, Subclass>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILocalizationService _loc;

        public SubclassService(IUnitOfWork uow, ILocalizationService loc)
        {
            _uow = uow;
            _loc = loc;
        }
        public async Task<SubclassDto> ArmDto(Subclass entity)
        {
            return new SubclassDto
            {
                Id = entity.Id,
                Name = await _loc.GetStringAsync(entity.Id, LocProperty.Name),
                Description = await _loc.GetStringAsync(entity.Id, LocProperty.Description),
                Progressions = entity.Progressions,
            };
        }

        public SubclassDto ArmDto(Subclass entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords)
        {
            var Name = localizedWords?.TryGetValue(LocProperty.Name, out var nameL) == true && nameL?.TryGetValue(entity.Id, out var entityName) == true ? entityName : "Uknown entity...";
            var Description = localizedWords?.TryGetValue(LocProperty.Description, out var descriptionL) == true && descriptionL?.TryGetValue(entity.Id, out var description) == true ? description : "Uknown entity...";

            return new SubclassDto
            {
                Id = entity.Id,
                Name = Name,
                Description = Description,
                Progressions = entity.Progressions,
            };
        }

        public async Task<List<SubclassDto>> GetAllAsync(Expression<Func<Subclass, bool>>? filter, Action<IncludeAggregator<Subclass>>? includes = null)
        {
            var classes = await _uow.Subclasses.GetAllAsync(filter, includes!);
            var classesLocations = await _loc.GetAllAsync(LocEntity.Class, [LocProperty.Name, LocProperty.Description]);
            var featuresNames = await _loc.GetTranslationsForLanguageAsync(LocEntity.Feature);

            var dtos = new List<SubclassDto>();
            foreach (var entity in classes)
            {
                var classDto = ArmDto(entity!, classesLocations);

                ArmFeatureDtos(classDto, featuresNames);

                dtos.Add(classDto);
            }

            return dtos.OrderBy(x => x.Name).ToList();
        }

        private void ArmFeatureDtos(SubclassDto classDto, List<LocalizedContent> featuresNames)
        {
            foreach (var pro in classDto.Progressions)
            {
                foreach (var feature in pro.Features)
                {
                    FeatureDto featureDto = new FeatureDto
                    {
                        Id = feature.Id,
                        Name = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Name)?.Text ?? "Sin nombre",
                        Description = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Description)?.Text ?? "Sin descripcion",
                        Special = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Name)?.Text ?? "Sin especial",
                        Modifiers = feature.Modifiers
                    };
                    classDto.FeatureDtos.Add(featureDto);
                }
            }
        }

        public async Task<SubclassDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Subclass>>? includes = null)
        {
            var race = await _uow.Subclasses.GetByIdAsync(id, includes);
            if (race == null) return null!;

            return await ArmDto(race);
        }

        public Task<List<SubclassDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
