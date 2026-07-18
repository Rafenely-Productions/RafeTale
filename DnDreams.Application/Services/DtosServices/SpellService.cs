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
    public class SpellService(IUnitOfWork uow, ILocalizationService loc) : IService<SpellDto, Spell>
    {
        public async Task<SpellDto> ArmDto(Spell spell)
        {

            return new SpellDto
            {
                Id = spell.Id,
                TechnicalName = spell.TechnicalName,
                Name = await loc.GetStringAsync(spell.Id, LocProperty.Name),
                Description = await loc.GetStringAsync(spell.Id, LocProperty.Description),
                MaterialComponentDescription = await loc.GetStringAsync(spell.Id, LocProperty.MaterialComponentDescription),
                School = spell.School,
                Level = spell.Level,

            };

        }

        public SpellDto ArmDto(Spell spell, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords)
        {
            var Name = localizedWords?.TryGetValue(LocProperty.Name, out var nameL) == true && nameL?.TryGetValue(spell.Id, out var spellName) == true ? spellName : "Uknown Spell...";
            var Description = localizedWords?.TryGetValue(LocProperty.Description, out var descriptionL) == true && descriptionL?.TryGetValue(spell.Id, out var description) == true ? description : "Uknown Spell...";
            var Material = localizedWords?.TryGetValue(LocProperty.MaterialComponentDescription, out var mateL) == true && mateL?.TryGetValue(spell.Id, out var matel) == true ? matel : "Uknown Spell...";

            return new SpellDto
            {
                Id = spell.Id,
                TechnicalName = spell.TechnicalName,
                Name = Name,
                Description = Description,
                MaterialComponentDescription = Material,
                Level = spell.Level
            };

        }

        public async Task<List<SpellDto>> GetAllAsync(Expression<Func<Spell, bool>>? filter, Action<IncludeAggregator<Spell>>? includes = null)
        {
            var spells = await uow.Spells.GetAllAsync(filter, includes!);
            var descriptions = await loc.GetAllAsync(LocEntity.Spell, [LocProperty.Name, LocProperty.Description, LocProperty.MaterialComponentDescription]);

            var spellDtos = new List<SpellDto>(spells.Count());
            foreach (var spell in spells.OrderBy(x => x!.Level))
            {
                spellDtos.Add(ArmDto(spell!, descriptions));
            }
            return [.. spellDtos.OrderBy(x => x.Level).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)];
        }

        public Task<List<SpellDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<SpellDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Spell>>? includes = null)
        {
            var spell = await uow.Spells.GetByIdAsync(id);
            if (spell == null)
                return null!;

            return await ArmDto(spell);
        }
    }
}