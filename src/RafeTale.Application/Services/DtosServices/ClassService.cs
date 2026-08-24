using DocumentFormat.OpenXml.Vml.Office;
using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Interfaces.DtosInterfaces;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Helpers;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;
using System.Linq.Expressions;

namespace RafeTale.Application.Services.DtosServices
{
    public class ClassService(IUnitOfWork uow, ILocalizationService loc) : IService<ClassDefinitionDto, ClassDefinition>
    {
        public async Task<ClassDefinitionDto> ArmDto(ClassDefinition entity)
        {
            // Traemos las traducciones específicas para las subclases e inicializamos las características
            var featuresNames = await loc.GetTranslationsForLanguageAsync(LocEntity.Feature);

            var dto = new ClassDefinitionDto
            {
                Id = entity.Id,
                TechnicalName = entity.TechnicalName,
                Name = await loc.GetStringAsync(entity.Id, LocProperty.Name),
                Description = await loc.GetStringAsync(entity.Id, LocProperty.Description),
                HitDie = entity.HitDie,
                HitDieValue = entity.HitDieValue,
                Progressions = ArmProgressionsDto(entity.Progressions),
                PrimaryAbility = [.. entity.PrimaryAbility.Select(l => l.ToString())],
                SavingThrowProficiencies = [.. entity.SavingThrowProficiencies.Select(l => l.ToString())],
                ArmorProficiencies = [.. entity.ArmorProficiencies.Select(l => l.ToString())],
                WeaponProficiencies = [.. entity.WeaponProficiencies.Select(l => l.ToString())],
                ToolProficiencies = [.. entity.ToolProficiencies.Select(l => l.ToString())],
                SkillProficiencies = [.. entity.SkillProficiencies.Select(l => l.TechnicalName?.ToString())],
                SkillToChoose = entity.SkillsToChoose,

            };

            // Cargamos las características de la clase base
            ArmFeatureDtos(dto, featuresNames);

            return dto;
        }

        public ClassDefinitionDto ArmDto(ClassDefinition entity, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords)
        {
            var name = localizedWords?.TryGetValue(LocProperty.Name, out var nameL) == true && nameL?.TryGetValue(entity.Id, out var entityName) == true ? entityName : "Unknown entity...";
            var description = localizedWords?.TryGetValue(LocProperty.Description, out var descriptionL) == true && descriptionL?.TryGetValue(entity.Id, out var entityDesc) == true ? entityDesc : "Unknown entity...";

            return new ClassDefinitionDto
            {
                Id = entity.Id,
                Name = name,
                TechnicalName = entity.TechnicalName,
                Description = description,
                HitDie = entity.HitDie,
                HitDieValue = entity.HitDieValue,
                Progressions = ArmProgressionsDto(entity.Progressions),
                PrimaryAbility = [.. entity.PrimaryAbility.Select(l => l.ToString())],
                SavingThrowProficiencies = [.. entity.SavingThrowProficiencies.Select(l => l.ToString())],
                ArmorProficiencies = [.. entity.ArmorProficiencies.Select(l => l.ToString())],
                WeaponProficiencies = [.. entity.WeaponProficiencies.Select(l => l.ToString())],
                ToolProficiencies = [.. entity.ToolProficiencies.Select(l => l.ToString())],
                SkillProficiencies = [.. entity.SkillProficiencies.Select(l => l.ToString())],
                SkillToChoose = entity.SkillsToChoose,

                // UNIÓN EN BULK: Reutilizamos el diccionario de palabras localizadas de la Clase 
                // ya que tu enum 'LocEntity.Class' engloba también a las Subclases+
                //Subclasses = entity.Subclasses?.Select(s => ArmSubclassDtoInMem(s, localizedWords)).ToList() ?? []
            };
        }
        public async Task<List<ClassDefinitionDto>> GetAllAsync(Expression<Func<ClassDefinition, bool>>? filter, Action<IncludeAggregator<ClassDefinition>>? includes = null)
        {
            var classes = await uow.ClassDefinitions.GetAllAsync(filter, includes);
            var classesLocations = await loc.GetAllAsync(LocEntity.Class, [LocProperty.Name, LocProperty.Description]);
            var subclasesNames = await loc.GetTranslationsForLanguageAsync(LocEntity.Subclass);
            var featuresNames = await loc.GetTranslationsForLanguageAsync(LocEntity.Feature);

            var dtos = new List<ClassDefinitionDto>();
            foreach (var entity in classes)
            {
                var classDto = ArmDto(entity!, classesLocations);

                // 1. Armamos las características de la Clase Base (Nivel 1 al 20)
                ArmFeatureDtos(classDto, featuresNames);
                ArmSubclasesDtos(entity!, classDto, subclasesNames, featuresNames);


                dtos.Add(classDto);
            }

            return [.. dtos.OrderBy(x => x.TechnicalName)];
        }
        private static void ArmSubclasesDtos(ClassDefinition classD, ClassDefinitionDto classDto, List<LocalizedContent> subclasesNames, List<LocalizedContent> featureNames)
        {
            if (classD.Subclasses == null) return;
            for (int i = 0; i < classD.Subclasses.Count; i++)
            {
                var sub = classD.Subclasses.ElementAt(i);
                var subDto = new SubclassDto
                {
                    Id = sub.Id,
                    Name = subclasesNames.FirstOrDefault(t => t.EntityId == sub.Id && t.Property == LocProperty.Name)?.Text ?? "Sin nombre",
                    Description = subclasesNames.FirstOrDefault(t => t.EntityId == sub.Id && t.Property == LocProperty.Description)?.Text ?? "Sin descripcion",
                    Progressions = ArmSubClassProgressionsDto(sub.Progressions)
                };
                ArmSubclassFeatureDtos(subDto, featureNames);
                classDto.Subclasses.Add(subDto);
            }
        }

        private static void ArmFeatureDtos(ClassDefinitionDto classDto, List<LocalizedContent> featuresNames)
        {
            if (classDto.Progressions == null) return;

            foreach (var pro in classDto.Progressions)
            {
                foreach (var feature in pro.Features)
                {
                    FeatureDto featureDto = new()
                    {
                        Id = feature.Id,
                        Name = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Name)?.Text ?? "Sin nombre",
                        Description = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Description)?.Text ?? "Sin descripcion",
                        //Special = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Name)?.Text ?? "Sin especial",
                        Modifiers = feature.Modifiers
                    };
                    classDto.FeatureDtos.Add(featureDto);
                }
            }
        }

        private static void ArmSubclassFeatureDtos(SubclassDto subclassDto, List<LocalizedContent> featuresNames)
        {
            if (subclassDto.Progressions == null) return;

            foreach (var pro in subclassDto.Progressions)
            {
                foreach (var feature in pro.Features)
                {
                    FeatureDto featureDto = new ()
                    {
                        Id = feature.Id,
                        Name = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Name)?.Text ?? "Sin nombre",
                        Description = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Description)?.Text ?? "Sin descripcion",
                        //Special = featuresNames.FirstOrDefault(t => t.EntityId == feature.Id && t.Property == LocProperty.Name)?.Text ?? "Sin especial",
                        Modifiers = feature.Modifiers
                    };
                    subclassDto.FeatureDtos.Add(featureDto);
                }
            }
        }

        public async Task<ClassDefinitionDto> GetByIdAsync(Guid id, Action<IncludeAggregator<ClassDefinition>>? includes = null)
        {
            var classDef = await uow.ClassDefinitions.GetByIdAsync(id, includes);
            if (classDef == null) return null!;
            var classDto = await ArmDto(classDef);
            var subclasesNames = await loc.GetTranslationsForLanguageAsync(LocEntity.Subclass);
            var featuresNames = await loc.GetTranslationsForLanguageAsync(LocEntity.Feature);
            ArmFeatureDtos(classDto, featuresNames);
            ArmSubclasesDtos(classDef!, classDto, subclasesNames, featuresNames);
            return classDto;
        }

        private static List<ClassLevelProgressionDto> ArmProgressionsDto(ICollection<ClassLevelProgression> progressions)
        {
            return [.. progressions.Select(p => new ClassLevelProgressionDto
            {
                Id = p.Id,
                Level = p.Level,
                Features = [.. p.Features.Select(f => new FeatureDto
                {
                    Id = f.Id,
                    Name = f.TechnicalName,
                    Modifiers = ArmModifier(f.Modifiers)
                })]
            })];
        }
        private static List<SubclassLevelProgressionDto> ArmSubClassProgressionsDto(ICollection<SubclassLevelProgression> progressions)
        {
            return [.. progressions.Select(p => new SubclassLevelProgressionDto
            {
                Id = p.Id,
                Level = p.Level,
                Features = [.. p.Features.Select(f => new FeatureDto
                {
                    Id = f.Id,
                    Name = f.TechnicalName,
                    Modifiers = ArmModifier(f.Modifiers)
                })]
            })];
        }

        private static List<ModifierDataDto> ArmModifier(List<ModifierData> modifier)
        {
            List<ModifierDataDto> modifiers = [];
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
    }
}