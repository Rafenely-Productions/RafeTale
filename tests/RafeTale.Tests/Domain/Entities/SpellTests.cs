using System;
using System.Collections.Generic;
using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using Xunit;

namespace RafeTale.Tests.Domain.Entities;

public class SpellTests
{
    [Fact]
    public void DefaultConstructor_InitializesCollections_CollectionsAreNotNull()
    {
        // Arrange & Act
        var spell = new Spell();

        // Assert
        spell.Components.Should().NotBeNull();
        spell.ClassesTechnicalNames.Should().NotBeNull();
    }

    [Fact]
    public void DefaultConstructor_TechnicalName_IsEmptyString()
    {
        // Arrange & Act
        var spell = new Spell();

        // Assert
        spell.TechnicalName.Should().BeEmpty();
    }

    [Theory]
    [InlineData(SpellLevel.Cantrip)]
    [InlineData(SpellLevel.Level1)]
    [InlineData(SpellLevel.Level9)]
    public void Level_BetweenZeroAndNine_ReturnsAssignedValue(SpellLevel level)
    {
        // Arrange
        var spell = new Spell { Level = level };

        // Act & Assert
        spell.Level.Should().Be(level);
    }

    [Fact]
    public void Cantrip_HasLevelZero()
    {
        // Arrange & Act
        var spell = new Spell { Level = SpellLevel.Cantrip };

        // Assert
        ((int)spell.Level).Should().Be(0);
    }

    [Fact]
    public void FirstLevelSpell_HasLevelOne()
    {
        // Arrange & Act
        var spell = new Spell { Level = SpellLevel.Level1 };

        // Assert
        ((int)spell.Level).Should().Be(1);
    }

    [Fact]
    public void DefaultConstructor_Concentration_IsNo()
    {
        // Arrange & Act
        var spell = new Spell();

        // Assert
        spell.Concentration.Should().Be(SpellConcentration.No);
    }

    [Fact]
    public void DefaultConstructor_Ritual_IsFalse()
    {
        // Arrange & Act
        var spell = new Spell();

        // Assert
        spell.Ritual.Should().BeFalse();
    }

    [Fact]
    public void SetSchoolOfMagic_School_ReturnsAssignedValue()
    {
        // Arrange
        var spell = new Spell
        {
            // Act
            School = SchoolOfMagicEnum.Evocation
        };

        // Assert
        spell.School.Should().Be(SchoolOfMagicEnum.Evocation);
    }

    [Fact]
    public void SetCastingTime_CastingTime_ReturnsAssignedValue()
    {
        // Arrange
        var spell = new Spell
        {
            // Act
            CastingTime = CastingTime.BonusAction
        };

        // Assert
        spell.CastingTime.Should().Be(CastingTime.BonusAction);
    }

    [Fact]
    public void SetRange_Range_ReturnsAssignedValue()
    {
        // Arrange
        var spell = new Spell
        {
            // Act
            Range = SpellRange.Touch
        };

        // Assert
        spell.Range.Should().Be(SpellRange.Touch);
    }

    [Fact]
    public void AddComponent_Components_ContainsTheComponent()
    {
        // Arrange
        var spell = new Spell();

        // Act
        spell.Components.Add(SpellComponent.V);

        // Assert
        spell.Components.Should().ContainSingle()
            .Which.Should().Be(SpellComponent.V);
    }

    [Fact]
    public void AddClass_ClassTechnicalName_ClassesTechnicalNames_ContainsTheClass()
    {
        // Arrange
        var spell = new Spell();
        const string className = "Wizard";

        // Act
        spell.ClassesTechnicalNames.Add(className);

        // Assert
        spell.ClassesTechnicalNames.Should().ContainSingle()
            .Which.Should().Be(className);
    }
}
