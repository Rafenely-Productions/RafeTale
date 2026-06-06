using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
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
                Name = await _loc.GetStringAsync(entity.Id, "Name"),
                Description = await _loc.GetStringAsync(entity.Id, "Description"),
                HitDie = entity.HitDie,
                HitDieValue = entity.HitDieValue,
                Progressions = entity.Progressions
            };
        }

        public async Task<List<ClassDefinitionDto>> GetAllAsync(Expression<Func<ClassDefinition, bool>>? filter, params Expression<Func<ClassDefinition, object>>[] includes)
        {
            var classes = await _uow.ClassDefinitions.GetClassesWithFeatures(filter, includes);
            var classesNames = await _loc.GetAllAsync(nameof(ClassDefinition), "Name");
            var featuresNames = await _loc.GetTranslationsForLanguageAsync(nameof(Feature));

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

                foreach (var pro in classDto.Progressions)
                {
                    foreach (var feature in pro.Features)
                    {
                        FeatureDto featureDto = new FeatureDto
                        {
                            Id = feature.Id,
                            Name = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == "Name")?.Text ?? "Sin nombre",
                            Description = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == "Description")?.Text ?? "Sin descripcion",
                            Special = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == "Special")?.Text ?? "Sin especial",
                            Modifiers = feature.Modifiers
                        };
                    classDto.FeatureDtos.Add(featureDto);
                    }
                }
                dtos.Add(classDto);
            }

            return dtos;
        }

        public async Task<ClassDefinitionDto> GetByIdAsync(Guid id, params Expression<Func<ClassDefinition, object>>[] includes)
        {
            var race = await _uow.ClassDefinitions.GetByIdAsync(id, includes);
            if (race == null) return null!;

            return await ArmDto(race);
        }
    }
}
