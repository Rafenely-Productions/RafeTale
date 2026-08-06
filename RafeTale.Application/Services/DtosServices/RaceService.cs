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
    public class RaceService(IUnitOfWork uow, ILocalizationService loc) : IService<RaceDto, Race>
    {
        public async Task<RaceDto> ArmDto(Race race)
        {
            var localizedSubraces = await loc.GetAllAsync(LocEntity.SubRace, [LocProperty.Name, LocProperty.Description]);
            var localizedTraits = await loc.GetAllAsync(LocEntity.Trait, [LocProperty.Name, LocProperty.Description]);

            return new RaceDto
            {
                Id = race.Id,
                Name = await loc.GetStringAsync(race.Id, LocProperty.Name),
                Description = await loc.GetStringAsync(race.Id, LocProperty.Description),
                Resistances = await loc.GetStringAsync(race.Id, LocProperty.Resistances),
                Size = race.Size,
                CreatureType = race.CreatureType,
                Speed = race.Speed,
                Languages = race.Languages,
                Traits = ArmTraitDtos(race.Traits, localizedTraits),
                SubRaces = ArmSubraceDtos(race.SubRaces, localizedSubraces, localizedTraits)
            };
        }

        public RaceDto ArmDto(Race race, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords)
        {
            return new RaceDto
            {
                Id = race.Id,
                Name = localizedWords != null && localizedWords.TryGetValue(LocProperty.Name, out var nameDict) && nameDict.TryGetValue(race.Id, out var n) ? n : "[No Name]",
                Description = localizedWords != null && localizedWords.TryGetValue(LocProperty.Description, out var descDict) && descDict.TryGetValue(race.Id, out var d) ? d : "[No Description]",
                Resistances = localizedWords != null && localizedWords.TryGetValue(LocProperty.Resistances, out var resDict) && resDict.TryGetValue(race.Id, out var r) ? r : "[No Resistances]",
                Size = race.Size,
                CreatureType = race.CreatureType,
                Speed = race.Speed,
                Languages = race.Languages,
                Traits = new(), // Se llenan en GetAllAsync usando el inicializador si hiciera falta
                SubRaces = new()
            };
        }

        public async Task<List<RaceDto>> GetAllAsync(Expression<Func<Race, bool>>? filter, Action<IncludeAggregator<Race>>? includes)
        {
            var races = await uow.Races.GetAllAsync(filter, includes);

            var localizedSubraces = await loc.GetAllAsync(LocEntity.SubRace, [LocProperty.Name, LocProperty.Description]);
            var localizedTraits = await loc.GetAllAsync(LocEntity.Trait, [LocProperty.Name, LocProperty.Description]);
            var localizedWords = await loc.GetAllAsync(LocEntity.Race, [LocProperty.Name, LocProperty.Description, LocProperty.Resistances]);

            var raceDtos = new List<RaceDto>();
            foreach (var race in races)
            {
                // Mapeamos el DTO base usando el inicializador 'with' o reconstruyendo
                var baseDto = ArmDto(race!, localizedWords);

                // Como son de tipo 'init', creamos el DTO definitivo inyectando las listas mapeadas desde un inicio
                var completeDto = new RaceDto
                {
                    Id = baseDto.Id,
                    Name = baseDto.Name,
                    Description = baseDto.Description,
                    Resistances = baseDto.Resistances,
                    Size = baseDto.Size,
                    CreatureType = baseDto.CreatureType,
                    Speed = baseDto.Speed,
                    Languages = baseDto.Languages,
                    Traits = ArmTraitDtos(race.Traits, localizedTraits),
                    SubRaces = ArmSubraceDtos(race.SubRaces, localizedSubraces, localizedTraits)
                };

                raceDtos.Add(completeDto);
            }
            return raceDtos;
        }

        public async Task<RaceDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Race>>? includes = null)
        {
            var race = await uow.Races.GetByIdAsync(id, includes);
            if (race == null) return null!;

            return await ArmDto(race);
        }


        private List<SubRaceDto> ArmSubraceDtos(List<SubRace>? subraces, Dictionary<LocProperty, Dictionary<Guid, string>> localizedSubraces, Dictionary<LocProperty, Dictionary<Guid, string>> localizedTraits)
        {
            if (subraces == null) return new();

            return subraces.Select(sr => new SubRaceDto
            {
                Id = sr.Id,
                TechnicalName = sr.TechnicalName,
                Name = localizedSubraces.TryGetValue(LocProperty.Name, out var nameDict) && nameDict.TryGetValue(sr.Id, out var n) ? n : sr.TechnicalName,
                Description = localizedSubraces.TryGetValue(LocProperty.Description, out var descDict) && descDict.TryGetValue(sr.Id, out var d) ? d : "No description",
                RaceId = sr.RaceId,
                Traits = ArmTraitDtos(sr.Traits, localizedTraits) // Retorna List<TraitDto> limpio
            }).ToList();
        }

        private List<TraitDto> ArmTraitDtos(List<Trait>? traits, Dictionary<LocProperty, Dictionary<Guid, string>> localizedTraits)
        {
            if (traits == null) return new();

            return traits.Select(t => new TraitDto
            {
                Id = t.Id,
                TechnicalName = t.TechnicalName,
                Name = localizedTraits.TryGetValue(LocProperty.Name, out var nameDict) && nameDict.TryGetValue(t.Id, out var n) ? n : t.TechnicalName,
                Description = localizedTraits.TryGetValue(LocProperty.Description, out var descDict) && descDict.TryGetValue(t.Id, out var d) ? d : "No description available",
                RequiredLevel = t.RequiredLevel,
                Modifiers = t.Modifiers,
                RaceId = t.RaceId,
                SubraceId = t.SubraceId
            }).ToList();
        }
    }
}