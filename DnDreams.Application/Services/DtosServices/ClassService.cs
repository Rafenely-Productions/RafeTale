using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
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

        // ====================================================================
        // 1. ArmDto Asíncrono (Usado en búsquedas individuales por ID)
        // ====================================================================
        public async Task<ClassDefinitionDto> ArmDto(ClassDefinition entity)
        {
            // Traemos las traducciones específicas para las subclases e inicializamos las características
            var subclassesLocations = await _loc.GetAllAsync(LocEntity.Subclass, [LocProperty.Name, LocProperty.Description]);
            var featuresNames = await _loc.GetTranslationsForLanguageAsync(LocEntity.Feature);

            var dto = new ClassDefinitionDto
            {
                Id = entity.Id,
                Name = await _loc.GetStringAsync(entity.Id, LocProperty.Name),
                Description = await _loc.GetStringAsync(entity.Id, LocProperty.Description),
                HitDie = entity.HitDie,
                HitDieValue = entity.HitDieValue,
                Progressions = entity.Progressions,
                PrimaryAbility = entity.PrimaryAbility,
                SavingThrowProficiencies = entity.SavingThrowProficiencies,
                ArmorProficiencies = entity.ArmorProficiencies,
                WeaponProficiencies = entity.WeaponProficiencies,
                ToolProficiencies = entity.ToolProficiencies,
                SkillProficiencies = entity.SkillProficiencies,
                SkillToChoose = entity.SkillsToChoose,

                // UNIÓN: Mapeamos las subclases localizándolas en memoria
                /*Subclasses = entity.Subclasses?.Select(s =>
                {
                    var subDto = ArmSubclassDtoInMem(s, subclassesLocations);
                    ArmSubclassFeatureDtos(subDto, featuresNames);
                    return subDto;
                }).ToList() ?? []*/
            };

            // Cargamos las características de la clase base
            ArmFeatureDtos(dto, featuresNames);

            return dto;
        }

        public ClassDefinitionDto ArmDto(ClassDefinition entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords)
        {
            var name = localizedWords?.TryGetValue(LocProperty.Name, out var nameL) == true && nameL?.TryGetValue(entity.Id, out var entityName) == true ? entityName : "Unknown entity...";
            var description = localizedWords?.TryGetValue(LocProperty.Description, out var descriptionL) == true && descriptionL?.TryGetValue(entity.Id, out var entityDesc) == true ? entityDesc : "Unknown entity...";

            return new ClassDefinitionDto
            {
                Id = entity.Id,
                Name = name,
                Description = description,
                HitDie = entity.HitDie,
                HitDieValue = entity.HitDieValue,
                Progressions = entity.Progressions,
                PrimaryAbility = entity.PrimaryAbility,
                SavingThrowProficiencies = entity.SavingThrowProficiencies,
                ArmorProficiencies = entity.ArmorProficiencies,
                WeaponProficiencies = entity.WeaponProficiencies,
                ToolProficiencies = entity.ToolProficiencies,
                SkillProficiencies = entity.SkillProficiencies,
                SkillToChoose = entity.SkillsToChoose,

                // UNIÓN EN BULK: Reutilizamos el diccionario de palabras localizadas de la Clase 
                // ya que tu enum 'LocEntity.Class' engloba también a las Subclases+
                //Subclasses = entity.Subclasses?.Select(s => ArmSubclassDtoInMem(s, localizedWords)).ToList() ?? []
            };
        }

        // ====================================================================
        // 3. GetAllAsync (Trae la lista de clases con todo e inyecta las features)
        // ====================================================================
        public async Task<List<ClassDefinitionDto>> GetAllAsync(Expression<Func<ClassDefinition, bool>>? filter, params Expression<Func<ClassDefinition, object>>[] includes)
        {
            var classes = await _uow.ClassDefinitions.GetClassesWithFeatures(filter, includes);
            var classesLocations = await _loc.GetAllAsync(LocEntity.Class, [LocProperty.Name, LocProperty.Description]);
            var subclasesNames = await _loc.GetTranslationsForLanguageAsync(LocEntity.Subclass);
            var featuresNames = await _loc.GetTranslationsForLanguageAsync(LocEntity.Feature);

            var dtos = new List<ClassDefinitionDto>();
            foreach (var entity in classes)
            {
                var classDto = ArmDto(entity, classesLocations);

                // 1. Armamos las características de la Clase Base (Nivel 1 al 20)
                ArmFeatureDtos(classDto, featuresNames);
                ArmSubclasesDtos(entity, classDto, subclasesNames, featuresNames);


                dtos.Add(classDto);
            }

            return dtos.OrderBy(x => x.Name).ToList();
        }
        private void ArmSubclasesDtos(ClassDefinition classD, ClassDefinitionDto classDto, List<LocalizedContent> subclasesNames,List<LocalizedContent> featureNames)
        {
            if (classD.Subclasses == null) return;
            for(int i = 0; i < classD.Subclasses.Count; i++)
            {
                    var sub = classD.Subclasses.ElementAt(i);
                    var subDto = new SubclassDto
                    {
                        Id = sub.Id,
                        Name = subclasesNames.FirstOrDefault(t => t.EntityId == sub.Id && t.Property == LocProperty.Name)?.Text ?? "Sin nombre",
                        Description = subclasesNames.FirstOrDefault(t => t.EntityId == sub.Id && t.Property == LocProperty.Description)?.Text ?? "Sin descripcion",
                        Progressions = sub.Progressions
                    };
                    ArmSubclassFeatureDtos(subDto, featureNames);
                    classDto.Subclasses.Add(subDto);
            }
        }

        private void ArmFeatureDtos(ClassDefinitionDto classDto, List<LocalizedContent> featuresNames)
        {
            if (classDto.Progressions == null) return;

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

        private void ArmSubclassFeatureDtos(SubclassDto subclassDto, List<LocalizedContent> featuresNames)
        {
            if (subclassDto.Progressions == null) return;

            foreach (var pro in subclassDto.Progressions)
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
                    subclassDto.FeatureDtos.Add(featureDto);
                }
            }
        }

        private SubclassDto ArmSubclassDtoInMem(Subclass entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords)
        {
            var subName = localizedWords?.TryGetValue(LocProperty.Name, out var nameL) == true && nameL?.TryGetValue(entity.Id, out var entityName) == true ? entityName : "Uknown entity...";
            var subDesc = localizedWords?.TryGetValue(LocProperty.Description, out var descL) == true && descL?.TryGetValue(entity.Id, out var entityDescription) == true ? entityDescription : "Uknown entity...";


            return new SubclassDto
            {
                Id = entity.Id,
                Name = subName,
                Description = subDesc,
                Progressions = entity.Progressions
            };
        }

        public async Task<ClassDefinitionDto> GetByIdAsync(Guid id, params Expression<Func<ClassDefinition, object>>[] includes)
        {
            var classDef = await _uow.ClassDefinitions.GetByIdAsync(id, includes);
            if (classDef == null) return null!;

            return await ArmDto(classDef);
        }
    }
}