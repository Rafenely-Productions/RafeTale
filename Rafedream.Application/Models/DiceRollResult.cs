namespace Rafedream.Application.Models;

public class DiceRollResult
{
    public int NumberOfDice { get; set; }
    public int DiceSides { get; set; }
    public int Modifier { get; set; }
    public List<int> IndividualRolls { get; set; } = new();

    // Calcula el total sumando los dados más el modificador
    public int Total => IndividualRolls.Sum() + Modifier;

    // Identifica si fue un 1 natural o un 20 natural (útil para d20)
    public bool IsCriticalHit => DiceSides == 20 && IndividualRolls.Count == 1 && IndividualRolls.First() == 20;
    public bool IsCriticalMiss => DiceSides == 20 && IndividualRolls.Count == 1 && IndividualRolls.First() == 1;
}