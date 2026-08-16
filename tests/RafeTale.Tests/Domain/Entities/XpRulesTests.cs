using System;
using RafeTale.Domain.Entities;

namespace RafeTale.Tests.Domain.Entities;

public class XpRulesTests
{
    [Fact]
    public void DefaultConstructor_Id_IsNotEmptyGuid()
    {
        // Arrange & Act
        var xpRule = new XpRules();

        // Assert
        xpRule.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DefaultConstructor_Level_IsZero()
    {
        // Arrange & Act
        var xpRule = new XpRules();

        // Assert
        xpRule.Level.Should().Be(0);
    }

    [Fact]
    public void DefaultConstructor_RequiredXp_IsZero()
    {
        // Arrange & Act
        var xpRule = new XpRules();

        // Assert
        xpRule.RequiredXp.Should().Be(0);
    }

    [Fact]
    public void DefaultConstructor_Bonus_IsZero()
    {
        // Arrange & Act
        var xpRule = new XpRules();

        // Assert
        xpRule.Bonus.Should().Be(0);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 300)]
    [InlineData(20, 355000)]
    public void SetValues_Properties_ReturnAssignedValues(int level, int requiredXp)
    {
        // Arrange
        var xpRule = new XpRules();

        // Act
        xpRule.Level = level;
        xpRule.RequiredXp = requiredXp;
        xpRule.Bonus = 2;

        // Assert
        xpRule.Level.Should().Be(level);
        xpRule.RequiredXp.Should().Be(requiredXp);
        xpRule.Bonus.Should().Be(2);
    }
}
