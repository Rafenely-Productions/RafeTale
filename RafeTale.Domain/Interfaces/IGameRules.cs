// RafeTale.Domain/Interfaces/IGameRules.cs
namespace RafeTale.Domain.Interfaces;

public interface IGameRules
{
    bool IsDiceCriticalSuccess(int diceSides, int rollValue);
    bool IsDiceCriticalFailure(int diceSides, int rollValue);
}