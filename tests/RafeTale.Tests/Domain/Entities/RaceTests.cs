using System;
using System.Collections.Generic;
using FluentAssertions;
using RafeTale.Domain.Entities;
using Xunit;

namespace RafeTale.Tests.Domain.Entities;

public class RaceTests
{
    [Fact]
    public void DefaultConstructor_InitializesCollections_CollectionsAreNotNull()
    {
        // Arrange & Act
        var race = new Race();

        // Assert
        race.Languages.Should().NotBeNull();
        race.SubRaces.Should().NotBeNull();
        race.Traits.Should().NotBeNull();
    }

    [Fact]
    public void DefaultConstructor_TechnicalName_IsEmptyString()
    {
        // Arrange & Act
        var race = new Race();

        // Assert
        race.TechnicalName.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_Speed_IsEmptyString()
    {
        // Arrange & Act
        var race = new Race();

        // Assert
        race.Speed.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_Traits_IsEmptyList()
    {
        // Arrange & Act
        var race = new Race();

        // Assert
        race.Traits.Should().BeEmpty();
    }

    [Fact]
    public void AddSubRace_SubRaces_ContainsTheSubRace()
    {
        // Arrange
        var race = new Race();
        var subRace = new SubRace { TechnicalName = "High Elf" };

        // Act
        race.SubRaces.Add(subRace);

        // Assert
        race.SubRaces.Should().ContainSingle()
            .Which.Should().Be(subRace);
    }

    [Fact]
    public void AddLanguage_Languages_ContainsTheLanguage()
    {
        // Arrange
        var race = new Race();
        var language = new Language { TechnicalName = "Common" };

        // Act
        race.Languages.Add(language);

        // Assert
        race.Languages.Should().ContainSingle()
            .Which.Should().Be(language);
    }

    [Fact]
    public void AddTrait_Traits_ContainsTheTrait()
    {
        // Arrange
        var race = new Race();
        var trait = new Trait { TechnicalName = "Darkvision" };

        // Act
        race.Traits.Add(trait);

        // Assert
        race.Traits.Should().ContainSingle()
            .Which.Should().Be(trait);
    }
}
