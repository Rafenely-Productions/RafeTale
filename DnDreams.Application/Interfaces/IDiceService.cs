using DnDreams.Application.Models;

namespace DnDreams.Application.Interfaces;

public interface IDiceService
{
    /// <summary>
    /// Realiza una tirada de dados estándar (ej. 2d6 + 3).
    /// </summary>
    DiceRollResult Roll(int numberOfDice, int sides, int modifier = 0);

    /// <summary>
    /// Realiza una tirada con Ventaja o Desventaja (tira 2d20 y se queda con el mayor o menor).
    /// </summary>
    DiceRollResult RollWithAdvantage(bool hasAdvantage, int modifier = 0);
}