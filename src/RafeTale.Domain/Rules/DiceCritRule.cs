namespace RafeTale.Domain.Rules;

public readonly record struct DiceCritRule(
    int DiceSides,
    int? CriticalSuccessValue,
    int? CriticalFailureValue);

