using FluentAssertions;
using RafeTale.Domain.Entities;
using Xunit;

namespace RafeTale.Tests.Domain.Entities;

public class BackgroundTests
{
    [Fact]
    public void DefaultConstructor_InitializesCollections_CollectionsAreNotNull()
    {
        // Arrange & Act
        var background = new Background();

        // Assert
        background.ASIs.Should().NotBeNull();
        background.SkillProficiencies.Should().NotBeNull();
    }

    [Fact]
    public void DefaultConstructor_TechnicalName_IsEmptyString()
    {
        // Arrange & Act
        var background = new Background();

        // Assert
        background.TechnicalName.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_Feat_IsNull()
    {
        // Arrange & Act
        var background = new Background();

        // Assert
        background.Feat.Should().BeNull();
    }

    [Fact]
    public void SetFeat_Feat_ReturnsAssignedValue()
    {
        // Arrange
        var background = new Background();
        var feat = new Feat { TechnicalName = "Magic Initiate" };

        // Act
        background.Feat = feat;

        // Assert
        background.Feat.Should().Be(feat);
    }
}
