using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services.DtosServices
{
    public class CharacterService : IService<CharacterDto, Character>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILocalizationService _loc;

        public CharacterService(IUnitOfWork uow, ILocalizationService loc)
        {
            _uow = uow;
            _loc = loc;
        }
        public async Task<List<CharacterDto>> GetAllAsync(Expression<Func<Character, bool>>? filter, params Expression<Func<Character, object>>[] includes)
        {
            var characters = await _uow.Characters.GetAllAsync(filter, includes);
            var names = await _loc.GetAllAsync("Character", "Name");
            var descriptions = await _loc.GetAllAsync("Character", "Description");

            var characterDtos = new List<CharacterDto>();
            foreach (var character in characters)
            {
                characterDtos.Add(new CharacterDto
                {
                    
                });
            }
            return characterDtos;
        }

        public async Task<CharacterDto> GetByIdAsync(Guid id, params Expression<Func<Character, object>>[] includes)
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
    }
}
