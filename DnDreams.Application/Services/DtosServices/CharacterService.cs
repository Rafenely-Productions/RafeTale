using DnDreams.Application.DTOs;
using DnDreams.Domain.Helpers;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services.DtosServices
{
    public class CharacterService(IUnitOfWork uow, ILocalizationService loc) : IService<CharacterDto, Character>
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly ILocalizationService _loc = loc;

        public async Task<List<CharacterDto>> GetAllAsync(Expression<Func<Character, bool>>? filter, Action<IncludeAggregator<Character>>? includes = null)
        {
            var characters = await _uow.Characters.GetAllAsync(filter, includes);
            //var names = await _loc.GetAllAsync(LocEntity.Character, LocProperty.Name);
            //var descriptions = await _loc.GetAllAsync(LocEntity.Character, LocProperty.Description);

            var characterDtos = new List<CharacterDto>();
            foreach (var character in characters)
            {
                characterDtos.Add(new CharacterDto
                {
                    
                });
            }
            return characterDtos;
        }

        public async Task<CharacterDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Character>>? includes = null)
        {
            var character = await _uow.Characters.GetByIdAsync(id);
            if (character == null) return null!;

            return await ArmDto(character);
        }

        public async Task<CharacterDto> ArmDto(Character race)
        {
            return new CharacterDto
            {
                
            };

        }

        public Task<CharacterDto> ArmDto(Character entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords)
        {
            throw new NotImplementedException();
        }

        CharacterDto IService<CharacterDto, Character>.ArmDto(Character entity, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords)
        {
            throw new NotImplementedException();
        }

        public Task<List<CharacterDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
