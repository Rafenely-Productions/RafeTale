using System;
using RafeTale.Domain.Entities;

namespace RafeTale.Tests.Domain.Entities;

public class CharacterSpellSlotsTests
{
    [Fact]
    public void DefaultConstructor_Ids_AreEmptyGuids()
    {
        // Arrange & Act
        var slots = new CharacterSpellSlots();

        // Assert
        slots.Id.Should().Be(Guid.Empty);
        slots.CharacterId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void DefaultConstructor_NumericValues_AreZero()
    {
        // Arrange & Act
        var slots = new CharacterSpellSlots();

        // Assert
        slots.Level.Should().Be(0);
        slots.MaxSlots.Should().Be(0);
        slots.UsedSlots.Should().Be(0);
    }

    [Theory]
    [InlineData(3, 0, 3)]
    [InlineData(3, 2, 1)]
    [InlineData(3, 3, 0)]
    [InlineData(3, 5, 0)]
    public void RemainingSlots_VariousScenarios_ReturnsExpectedValue(int maxSlots, int usedSlots, int expected)
    {
        // Arrange
        var slots = new CharacterSpellSlots
        {
            MaxSlots = maxSlots,
            UsedSlots = usedSlots
        };

        // Act & Assert
        slots.RemainingSlots.Should().Be(expected);
    }

    [Fact]
    public void SetCharacter_Character_ReturnsAssignedValue()
    {
        // Arrange
        var slots = new CharacterSpellSlots();
        var character = new Character { Name = "Gandalf" };

        // Act
        slots.Character = character;

        // Assert
        slots.Character.Should().Be(character);
    }
}
