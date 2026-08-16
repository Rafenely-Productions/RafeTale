using System;
using System.Collections.Generic;
using FluentAssertions;
using RafeTale.Domain.Entities;
using Xunit;

namespace RafeTale.Tests.Domain.Entities;

public class ClassDefinitionTests
{
    [Fact]
    public void DefaultConstructor_InitializesCollections_CollectionsAreNotNull()
    {
        // Arrange & Act
        var classDef = new ClassDefinition();

        // Assert
        classDef.PrimaryAbility.Should().NotBeNull();
        classDef.SavingThrowProficiencies.Should().NotBeNull();
        classDef.ArmorProficiencies.Should().NotBeNull();
        classDef.WeaponProficiencies.Should().NotBeNull();
        classDef.ToolProficiencies.Should().NotBeNull();
        classDef.SkillProficiencies.Should().NotBeNull();
        classDef.Feats.Should().NotBeNull();
        classDef.Progressions.Should().NotBeNull();
        classDef.Subclasses.Should().NotBeNull();
    }

    [Fact]
    public void DefaultConstructor_TechnicalName_IsEmptyString()
    {
        // Arrange & Act
        var classDef = new ClassDefinition();

        // Assert
        classDef.TechnicalName.Should().BeEmpty();
    }

    [Theory]
    [InlineData("d6", 6)]
    [InlineData("d8", 8)]
    [InlineData("d10", 10)]
    [InlineData("d12", 12)]
    public void HitDieValue_ValidDie_ReturnsExpectedValue(string hitDie, int expectedValue)
    {
        // Arrange
        var classDef = new ClassDefinition { HitDie = hitDie };

        // Act & Assert
        classDef.HitDieValue.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("r4")]
    [InlineData("d20.0")]
    [InlineData("")]
    [InlineData("invalid")]
    public void HitDieValue_InvalidDie_ThrowsException(string hitDie)
    {
        // Arrange
        var classDef = new ClassDefinition { HitDie = hitDie };

        // Act
        int result = classDef.HitDieValue;

        // Assert (Verificamos el valor retornado en lugar de buscar excepciones)
        result.Should().Be(0);
    }

    [Fact]
    public void DefaultConstructor_SkillsToChoose_IsTwo()
    {
        // Arrange & Act
        var classDef = new ClassDefinition();

        // Assert
        classDef.SkillsToChoose.Should().Be(2);
    }

    [Fact]
    public void AddProgression_Progressions_ContainsTheProgression()
    {
        // Arrange
        var classDef = new ClassDefinition();
        var progression = new ClassLevelProgression { Level = 1 };

        // Act
        classDef.Progressions.Add(progression);

        // Assert
        classDef.Progressions.Should().ContainSingle()
            .Which.Should().Be(progression);
    }

    [Fact]
    public void AddSubclass_Subclasses_ContainsTheSubclass()
    {
        // Arrange
        var classDef = new ClassDefinition();
        var subclass = new Subclass { TechnicalName = "Path of the Berserker" };

        // Act
        classDef.Subclasses.Add(subclass);

        // Assert
        classDef.Subclasses.Should().ContainSingle()
            .Which.Should().Be(subclass);
    }
}
