using RafeTale.Application.Interfaces;
using RafeTale.Application.Models;

namespace RafeTale.Application.Services;

public class DiceService : IDiceService
{
    public DiceRollResult Roll(int numberOfDice, int sides, int modifier = 0)
    {
        if (numberOfDice <= 0) throw new ArgumentException("El número de dados debe ser mayor a cero.", nameof(numberOfDice));
        if (sides <= 0) throw new ArgumentException("El número de caras debe ser mayor a cero.", nameof(sides));

        var rolls = new List<int>(numberOfDice);
        for (int i = 0; i < numberOfDice; i++)
        {
            rolls.Add(Random.Shared.Next(1, sides + 1));
        }

        return new DiceRollResult
        {
            NumberOfDice = numberOfDice,
            DiceSides = sides,
            Modifier = modifier,
            IndividualRolls = rolls
        };
    }

    public DiceRollResult RollWithAdvantage(bool hasAdvantage, int modifier = 0)
    {
        var roll1 = Random.Shared.Next(1, 21);
        var roll2 = Random.Shared.Next(1, 21);

        var finalRoll = hasAdvantage ? Math.Max(roll1, roll2) : Math.Min(roll1, roll2);

        return new DiceRollResult
        {
            NumberOfDice = 1,
            DiceSides = 20,
            Modifier = modifier,
            IndividualRolls = new List<int> { finalRoll }
        };
    }
}