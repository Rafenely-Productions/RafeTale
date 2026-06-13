using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Linq.Expressions;

namespace DnDreams.Application.Services.DtosServices
{
    public class ClassService : IService<ClassDefinitionDto, ClassDefinition>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILocalizationService _loc;

        public ClassService(IUnitOfWork uow, ILocalizationService loc)
        {
            _uow = uow;
            _loc = loc;
        }
        public async Task<ClassDefinitionDto> ArmDto(ClassDefinition entity)
        {
            return new ClassDefinitionDto
            {
                Id = entity.Id,
                Name = await _loc.GetStringAsync(entity.Id, LocProperty.Name),
                Description = await _loc.GetStringAsync(entity.Id, LocProperty.Description),
                HitDie = entity.HitDie,
                HitDieValue = entity.HitDieValue,
                Progressions = entity.Progressions
            };
        }

        public ClassDefinitionDto ArmDto(ClassDefinition entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords)
        {
            var Name = localizedWords?.TryGetValue(LocProperty.Name, out var nameL) == true && nameL?.TryGetValue(entity.Id, out var entityName) == true ? entityName : "Uknown entity...";
            var Description = localizedWords?.TryGetValue(LocProperty.Description, out var descriptionL) == true && descriptionL?.TryGetValue(entity.Id, out var description) == true ? description : "Uknown entity...";

            return new ClassDefinitionDto
            {
                Id = entity.Id,
                Name = Name,
                Description = Description,
                HitDie = entity.HitDie,
                HitDieValue = entity.HitDieValue,
                Progressions = entity.Progressions
            };
        }

        public async Task<List<ClassDefinitionDto>> GetAllAsync(Expression<Func<ClassDefinition, bool>>? filter, params Expression<Func<ClassDefinition, object>>[] includes)
        {
            var classes = await _uow.ClassDefinitions.GetClassesWithFeatures(filter, includes);
            var classesNames = await _loc.GetAllAsync(LocEntity.Class, LocProperty.Name);
            var featuresNames = await _loc.GetTranslationsForLanguageAsync(LocEntity.Feature);

            var dtos = new List<ClassDefinitionDto>();
            foreach (var entity in classes)
            {
                var classDto = new ClassDefinitionDto
                {
                    Id = entity.Id,
                    Name = classesNames.TryGetValue(entity.Id, out var n) ? n : "[No Name]",
                    HitDie = entity.HitDie,
                    HitDieValue = entity.HitDieValue,
                    Progressions = entity.Progressions,
                };

                ArmFeatureDtos(classDto, featuresNames);
                
                dtos.Add(classDto);
            }

            return dtos;
        }

        private void ArmFeatureDtos(ClassDefinitionDto classDto, List<LocalizedContent> featuresNames)
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

        public async Task<ClassDefinitionDto> GetByIdAsync(Guid id, params Expression<Func<ClassDefinition, object>>[] includes)
        {
            var race = await _uow.ClassDefinitions.GetByIdAsync(id, includes);
            if (race == null) return null!;

            return await ArmDto(race);
        }

        ClassDefinitionDto IService<ClassDefinitionDto, ClassDefinition>.ArmDto(ClassDefinition entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords)
        {
            throw new NotImplementedException();
        }
    }
}
