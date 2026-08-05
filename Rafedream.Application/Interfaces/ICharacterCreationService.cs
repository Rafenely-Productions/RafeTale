using Rafedream.Domain.Entities;
using Rafedream.Domain.Enums;
using Rafedream.Domain.Interfaces;

namespace Rafedream.Application.Interfaces;

public interface ICharacterCreationService
{
    Guid? SelectedRaceId { get; set; }
    Guid? SelectedClassId { get; set; }
    Guid? SelectedBackgroundId { get; set; }
    string Name { get; set; }
    string History { get; set; }
    Dictionary<ASI, int> BaseStats { get; set; }
    Dictionary<ASI, int> BonusStats { get; set; }

    void Reset();
    Task<Character> CreateAndSaveCharacterAsync(IUnitOfWork uow);
}