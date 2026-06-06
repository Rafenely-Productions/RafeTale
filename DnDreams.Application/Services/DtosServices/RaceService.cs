using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using System.Linq.Expressions;

namespace DnDreams.Application.Services.DtosServices
{
    public class RaceService : IService<RaceDto,Race>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILocalizationService _loc;

        public RaceService(IUnitOfWork uow, ILocalizationService loc)
        {
            _uow = uow;
            _loc = loc;
        }

        public async Task<RaceDto> ArmDto(Race race)
        {
            return new RaceDto 
            {
                Id = race.Id,
                Name = await _loc.GetStringAsync(race.Id, "Name"),
                Description = await _loc.GetStringAsync(race.Id, "Description"),
                Resistances = await _loc.GetStringAsync(race.Id, "Resistances"),
                Darkvision = race.Darkvision,
                Size = race.Size,
                CreatureType = race.CreatureType,
                Speed = race.Speed,
                Languages = race.Languages,
                SubRaces = race.SubRaces,
                Traits = race.Traits
            };

        }

        public async Task<List<RaceDto>> GetAllAsync(Expression<Func<Race, bool>>? filter,params Expression<Func<Race, object>>[] includes)
        {
            var races = await _uow.Races.GetAllAsync(filter,includes);
            var names = await _loc.GetAllAsync("Race", "Name");
            var descriptions = await _loc.GetAllAsync("Race", "Description");
            var resistances = await _loc.GetAllAsync("Race", "Resistances");

            var raceDtos = new List<RaceDto>();
            foreach (var race in races)
            {
                raceDtos.Add(new RaceDto
                {
                    Id = race.Id,
                    Name = names.TryGetValue(race.Id, out var n) ? n : "[No Name]",
                    Description = descriptions.TryGetValue(race.Id, out var d) ? d : "[No Description]",
                    Resistances = resistances.TryGetValue(race.Id, out var r) ? r : "[No Resistances]",
                    Darkvision = race.Darkvision,
                    Size = race.Size,
                    CreatureType = race.CreatureType,
                    Speed = race.Speed,
                    Languages = race.Languages,
                    SubRaces = race.SubRaces,
                    Traits = race.Traits
                });
            }
            return raceDtos;
        }

        public async Task<RaceDto> GetByIdAsync(Guid id, params Expression<Func<Race, object>>[] includes)
        {
            var race = await _uow.Races.GetByIdAsync(id);
            if (race==null)return null!;

            return await ArmDto(race);
        }
    }
}
