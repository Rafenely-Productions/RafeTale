using Rafedream.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Rafedream.Application.Interfaces;

public interface ILevelingService
{
    Task<bool> AddExperienceAsync(Guid characterId, int xpAmount);
    Task<bool> CommitLevelUpAsync(Guid characterId, int chosenHp, List<CharacterModifier> chosenModifiers, List<Guid> chosenFeatIds, List<Guid> chosenSpellIds);
}