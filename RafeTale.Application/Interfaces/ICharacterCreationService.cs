using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;

namespace RafeTale.Application.Interfaces;

public interface ICharacterCreationService
{
    Guid? SelectedRaceId { get; set; }
    Guid? SelectedClassId { get; set; }
    Guid? SelectedBackgroundId { get; set; }
    string Name { get; set; }
    string History { get; set; }
    Dictionary<AttributeImprovementChoice, int> BaseStats { get; set; }
    Dictionary<AttributeImprovementChoice, int> BonusStats { get; set; }

    void Reset();
    Task<Character> CreateAndSaveCharacterAsync(IUnitOfWork uow);
}