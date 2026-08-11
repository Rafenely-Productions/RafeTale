using RafeTale.Domain.Interfaces;
namespace RafeTale.Application.Models;

public class DiceRollResult
{
    public int NumberOfDice { get; set; }
    public int DiceSides { get; set; }
    public int Modifier { get; set; }
    public List<int> IndividualRolls { get; set; } = [];

    // Calcula el total sumando los dados más el modificador
    public int Total => IndividualRolls.Sum() + Modifier;

    public IGameRules? GameRules { get; set; }


    public bool IsCriticalHit =>
            IndividualRolls.Count == 1 &&
            (GameRules?.IsDiceCriticalSuccess(DiceSides, IndividualRolls.First())
             ?? (DiceSides == 20 && IndividualRolls.First() == 20));
    public bool IsCriticalMiss =>
            IndividualRolls.Count == 1 &&
            (GameRules?.IsDiceCriticalFailure(DiceSides, IndividualRolls.First())
             ?? (DiceSides == 20 && IndividualRolls.First() == 1));
}