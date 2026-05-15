using DnDreams.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace DnDreams.Application.Interfaces;

public interface ILevelingService
{
    Task<bool> AddExperienceAsync(Guid characterId, int xpAmount);
    Task<bool> CommitLevelUpAsync(Guid characterId, int chosenHp, List<CharacterModifier> chosenModifiers, List<Guid> chosenFeatIds, List<Guid> chosenSpellIds);
}