
using RafeTale.Domain.Interfaces;

namespace RafeTale.Domain.Rules;

public class DefaultGameRules : IGameRules
{
    private static readonly IReadOnlyDictionary<int, DiceCritRule> DefaultRules = new Dictionary<int, DiceCritRule>
    {
        { 4, new DiceCritRule(4, 4, 1) },
        { 6, new DiceCritRule(6, 6, 1) },
        { 8, new DiceCritRule(8, 8, 1) },
        { 10, new DiceCritRule(10, 10, 1) },
        { 12, new DiceCritRule(12, 12, 1) },
        { 20, new DiceCritRule(20, 20, 1) },
        { 100, new DiceCritRule(100, 100, 1) }
    };

    protected virtual IReadOnlyDictionary<int, DiceCritRule> Rules => DefaultRules;

    public bool IsDiceCriticalSuccess(int diceSides, int rollValue) =>
        Rules.TryGetValue(diceSides, out var rule) &&
        rule.CriticalSuccessValue.HasValue &&
        rollValue == rule.CriticalSuccessValue.Value;

    public bool IsDiceCriticalFailure(int diceSides, int rollValue) =>
        Rules.TryGetValue(diceSides, out var rule) &&
        rule.CriticalFailureValue.HasValue &&
        rollValue == rule.CriticalFailureValue.Value;
}