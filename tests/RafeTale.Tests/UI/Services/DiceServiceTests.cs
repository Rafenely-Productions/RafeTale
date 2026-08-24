using FluentAssertions;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Models;
using RafeTale.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RafeTale.Tests.UI.Services;

public class DiceServiceTests
{
    private readonly DiceService _diceService = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Roll_WithInvalidNumberOfDice_ShouldThrowArgumentException(int numberOfDice)
    {
        Action act = () => _diceService.Roll(numberOfDice, 6);

        act.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(numberOfDice));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Roll_WithInvalidSides_ShouldThrowArgumentException(int sides)
    {
        Action act = () => _diceService.Roll(1, sides);

        act.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(sides));
    }

    [Theory]
    [InlineData(1, 6, 0)]
    [InlineData(2, 8, 3)]
    [InlineData(5, 20, -2)]
    [InlineData(10, 4, 5)]
    public void Roll_ShouldReturnResultWithCorrectMetadata(int numberOfDice, int sides, int modifier)
    {
        var result = _diceService.Roll(numberOfDice, sides, modifier);

        result.Should().NotBeNull();
        result.NumberOfDice.Should().Be(numberOfDice);
        result.DiceSides.Should().Be(sides);
        result.Modifier.Should().Be(modifier);
        result.IndividualRolls.Should().HaveCount(numberOfDice);
    }

    [Theory]
    [InlineData(100, 6)]
    [InlineData(50, 20)]
    [InlineData(200, 4)]
    public void Roll_ShouldReturnRollsWithinValidRange(int numberOfDice, int sides)
    {
        var result = _diceService.Roll(numberOfDice, sides);

        result.IndividualRolls.Should().OnlyContain(roll => roll >= 1 && roll <= sides);
    }

    [Theory]
    [InlineData(3, 6, 4)]
    [InlineData(2, 10, -3)]
    [InlineData(5, 8, 0)]
    public void Roll_Total_ShouldEqualSumOfRollsPlusModifier(int numberOfDice, int sides, int modifier)
    {
        var result = _diceService.Roll(numberOfDice, sides, modifier);

        var expectedTotal = result.IndividualRolls.Sum() + modifier;
        result.Total.Should().Be(expectedTotal);
    }

    [Fact]
    public void Roll_CriticalHit_ShouldBeTrueForNatural20()
    {
        var result = new DiceRollResult
        {
            NumberOfDice = 1,
            DiceSides = 20,
            IndividualRolls = [20]
        };

        result.IsCriticalHit.Should().BeTrue();
        result.IsCriticalMiss.Should().BeFalse();
    }

    [Fact]
    public void Roll_CriticalMiss_ShouldBeTrueForNatural1()
    {
        var result = new DiceRollResult
        {
            NumberOfDice = 1,
            DiceSides = 20,
            IndividualRolls = [1]
        };

        result.IsCriticalMiss.Should().BeTrue();
        result.IsCriticalHit.Should().BeFalse();
    }

    [Theory]
    [InlineData(6, 1, 6)]
    public void Roll_CriticalHit_ShouldBeFalse_WhenConditionsNotMet(int sides, int numberOfDice, int rollValue)
    {
        var result = new DiceRollResult
        {
            NumberOfDice = numberOfDice,
            DiceSides = sides,
            IndividualRolls = [.. Enumerable.Repeat(rollValue, numberOfDice)]
        };

        result.IsCriticalHit.Should().BeFalse();
    }

    [Fact]
    public void Roll_ShouldEventuallyProduceCriticalHitAndMiss()
    {
        const int maxAttempts = 2000;
        bool foundHit = false;
        bool foundMiss = false;

        for (int i = 0; i < maxAttempts && !(foundHit && foundMiss); i++)
        {
            var result = _diceService.Roll(1, 20);
            if (result.IsCriticalHit) foundHit = true;
            if (result.IsCriticalMiss) foundMiss = true;
        }

        foundHit.Should().BeTrue("a d20 should produce a natural 20 within a reasonable number of rolls");
        foundMiss.Should().BeTrue("a d20 should produce a natural 1 within a reasonable number of rolls");
    }

    [Fact]
    public void RollWithAdvantage_ShouldReturnSingleD20Result()
    {
        var result = _diceService.RollWithAdvantage(true);

        result.Should().NotBeNull();
        result.NumberOfDice.Should().Be(1);
        result.DiceSides.Should().Be(20);
        result.IndividualRolls.Should().HaveCount(1);
        result.IndividualRolls.First().Should().BeInRange(1, 20);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(-2)]
    [InlineData(0)]
    public void RollWithAdvantage_ShouldApplyModifier(int modifier)
    {
        var result = _diceService.RollWithAdvantage(true, modifier);

        result.Modifier.Should().Be(modifier);
        result.Total.Should().Be(result.IndividualRolls.Sum() + modifier);
    }

    [Fact]
    public void RollWithAdvantage_ShouldOnlyReturnValuesBetween1And20()
    {
        for (int i = 0; i < 100; i++)
        {
            var advantage = _diceService.RollWithAdvantage(true);
            var disadvantage = _diceService.RollWithAdvantage(false);

            advantage.IndividualRolls.First().Should().BeInRange(1, 20);
            disadvantage.IndividualRolls.First().Should().BeInRange(1, 20);
        }
    }
}