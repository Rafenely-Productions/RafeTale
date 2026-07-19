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
    public class CharacterService(
        IUnitOfWork uow,
        ILocalizationService loc,
        IService<RaceDto, Race> raceService,
        IService<ClassDefinitionDto, ClassDefinition> classService,
        IService<BackgroundDto, Background> bgService,
        IService<SpellDto, Spell> spellService) : IService<CharacterDto, Character>
    {
        public async Task<CharacterDto> ArmDto(Character character)
        {
            // 1. Resolver los DTOs de sus orígenes usando tus servicios existentes
            var raceDto = await raceService.GetByIdAsync(character.RaceId);

            // Usamos los includes de progresiones y features para la clase
            var classDto = await classService.GetByIdAsync(character.ClassDefId, config => config
                .Include(x => x.SkillProficiencies)
                .IncludeCollection(x => x.Progressions, p => p.Features));

            var bgDto = await bgService.GetByIdAsync(character.BackgroundId);

            // 2. Traer traducciones de las características del personaje
            var localizedFeatures = await loc.GetAllAsync(LocEntity.Feature, [LocProperty.Name, LocProperty.Description]);

            var mappedFeatures = new List<FeatureDto>();
            foreach (var f in character.AcquiredFeatures)
            {
                mappedFeatures.Add(new FeatureDto
                {
                    Id = f.Id,
                    Name = localizedFeatures.TryGetValue(LocProperty.Name, out var nDict) && nDict.TryGetValue(f.Id, out var name) ? name : f.TechnicalName,
                    Description = localizedFeatures.TryGetValue(LocProperty.Description, out var dDict) && dDict.TryGetValue(f.Id, out var desc) ? desc : "No Description"
                });
            }
            var spellDtos = new List<SpellDto>();
            var localizedSpells = await loc.GetAllAsync(LocEntity.Spell, [LocProperty.Name, LocProperty.Description, LocProperty.MaterialComponentDescription]);

            foreach (var spell in character.KnownSpells)
            {
                var spellDto = spellService.ArmDto(spell, localizedSpells);
                spellDtos.Add(spellDto);
            }
            // 3. Ensamblar el DTO definitivo
            return new CharacterDto
            {
                Id = character.Id,
                Name = character.Name,
                History = character.History,
                Strength = character.Strength,
                Dexterity = character.Dexterity,
                Constitution = character.Constitution,
                Intelligence = character.Intelligence,
                Wisdom = character.Wisdom,
                Charisma = character.Charisma,
                MaxHp = character.MaxHp,
                CurrentHp = character.CurrentHp,
                Level = character.Level,
                Experience = character.Experience,
                Race = raceDto,
                ClassDef = classDto,
                Background = bgDto,
                AcquiredFeatures = mappedFeatures,
                SpellSlots = character.SpellSlots,
                KnownSpells = spellDtos
            };
        }

        public CharacterDto ArmDto(Character entity, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords)
        {
            // Implementación puente síncrona si se requiere para listas masivas
            return null!;
        }

        public async Task<List<CharacterDto>> GetAllAsync(Expression<Func<Character, bool>>? filter, Action<IncludeAggregator<Character>>? includes)
        {
            var characters = await uow.Characters.GetAllAsync(filter, includes);
            var dtos = new List<CharacterDto>();
            foreach (var c in characters)
            {
                dtos.Add(await ArmDto(c!));
            }
            return dtos;
        }

        public async Task<CharacterDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Character>>? includes = null)
        {
            // Forzamos los includes necesarios para el grafo relacional
            var character = await uow.Characters.GetByIdAsync(id, config =>
            {
                config.Include(c => c.ClassDef)
                .Include(c => c.AcquiredFeatures)
                .Include(c => c.SpellSlots);
                includes?.Invoke(config);
            });
            if (character == null) return null!;
            return await ArmDto(character);
        }
    }
}