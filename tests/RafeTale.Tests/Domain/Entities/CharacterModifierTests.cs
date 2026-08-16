using System;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;

namespace RafeTale.Tests.Domain.Entities;

public class CharacterModifierTests
{
    [Fact]
    public void DefaultConstructor_Source_IsEmptyString()
    {
        // Arrange & Act
        var modifier = new CharacterModifier();

        // Assert
        modifier.Source.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_Target_IsEmptyString()
    {
        // Arrange & Act
        var modifier = new CharacterModifier();

        // Assert
        modifier.Target.Should().BeEmpty();
    }

    [Fact]
    public void DefaultConstructor_Value_IsZero()
    {
        // Arrange & Act
        var modifier = new CharacterModifier();

        // Assert
        modifier.Value.Should().Be(0);
    }

    [Fact]
    public void DefaultConstructor_CharacterId_IsEmptyGuid()
    {
        // Arrange & Act
        var modifier = new CharacterModifier();

        // Assert
        modifier.CharacterId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void SetType_Type_ReturnsAssignedValue()
    {
        // Arrange
        var modifier = new CharacterModifier();

        // Act
        modifier.Type = ModifierType.AttributeBonus;

        // Assert
        modifier.Type.Should().Be(ModifierType.AttributeBonus);
    }

    [Fact]
    public void SetTarget_Target_ReturnsAssignedValue()
    {
        // Arrange
        var modifier = new CharacterModifier();

        // Act
        modifier.Target = "Strength";

        // Assert
        modifier.Target.Should().Be("Strength");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(-1)]
    [InlineData(0)]
    public void SetValue_Value_ReturnsAssignedValue(int value)
    {
        // Arrange
        var modifier = new CharacterModifier();

        // Act
        modifier.Value = value;

        // Assert
        modifier.Value.Should().Be(value);
    }

    [Fact]
    public void SetSource_Source_ReturnsAssignedValue()
    {
        // Arrange
        var modifier = new CharacterModifier();
        const string source = "Feat: Actor";

        // Act
        modifier.Source = source;

        // Assert
        modifier.Source.Should().Be(source);
    }
}
