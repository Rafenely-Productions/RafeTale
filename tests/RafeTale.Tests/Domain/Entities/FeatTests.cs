using System;
using System.Collections.Generic;
using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using Xunit;

namespace RafeTale.Tests.Domain.Entities;

public class FeatTests
{
    [Fact]
    public void DefaultConstructor_InitializesCollections_CollectionsAreNotNull()
    {
        // Arrange & Act
        var feat = new Feat();

        // Assert
        feat.Prerequisite.Should().NotBeNull();
        feat.Modifiers.Should().NotBeNull();
    }

    [Fact]
    public void DefaultConstructor_TechnicalName_IsEmptyString()
    {
        // Arrange & Act
        var feat = new Feat();

        // Assert
        feat.TechnicalName.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_Category_IsGeneral()
    {
        // Arrange & Act
        var feat = new Feat();

        // Assert
        feat.Category.Should().Be(CategoryFeat.General);
    }

    [Fact]
    public void DefaultConstructor_Modifiers_IsEmptyList()
    {
        // Arrange & Act
        var feat = new Feat();

        // Assert
        feat.Modifiers.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_Prerequisite_IsEmptyList()
    {
        // Arrange & Act
        var feat = new Feat();

        // Assert
        feat.Prerequisite.Should().BeEmpty();
    }

    [Fact]
    public void SetName_TechnicalName_ReturnsAssignedValue()
    {
        // Arrange
        var feat = new Feat();
        const string expectedName = "Toughness";

        // Act
        feat.TechnicalName = expectedName;

        // Assert
        feat.TechnicalName.Should().Be(expectedName);
    }

    [Fact]
    public void SetCategory_Category_ReturnsAssignedValue()
    {
        // Arrange
        var feat = new Feat
        {
            // Act
            Category = CategoryFeat.CombatStyle
        };

        // Assert
        feat.Category.Should().Be(CategoryFeat.CombatStyle);
    }
}
