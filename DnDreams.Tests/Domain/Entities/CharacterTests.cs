using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;

namespace DnDreams.Tests.Domain.Entities;

public class CharacterTests
{
    [Theory]
    [InlineData(1, -5)]
    [InlineData(9, -1)]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 1)]
    [InlineData(15, 2)]
    [InlineData(18, 4)]
    [InlineData(20, 5)]
    public void CalculateModifier_ReturnsCorrectValue(int score, int expectedModifier)
    {
        // Arrange
        var character = new Character();

        // Act
        int modifier = character.StrModifier; // Usamos la propiedad que llama CalculateModifier internamente
        // Pero para testear directamente, accedemos via Reflection o seteamos el stat
        character.Strength = score;

        // Assert
        character.StrModifier.Should().Be(expectedModifier);
    }

    [Fact]
    public void NewCharacter_HasDefaultStatsOf10()
    {
        // Arrange & Act
        var character = new Character();

        // Assert
        character.Strength.Should().Be(10);
        character.Dexterity.Should().Be(10);
        character.Constitution.Should().Be(10);
        character.Intelligence.Should().Be(10);
        character.Wisdom.Should().Be(10);
        character.Charisma.Should().Be(10);
    }

    [Fact]
    public void Character_WithModifier_AppliesBonusToStrength()
    {
        // Arrange
        var character = new Character
        {
            Strength = 14,
            CharacterModifiers = new List<CharacterModifier>
            {
                new()
                {
                    Type = ModifierType.AttributeBonus,
                    Target = TargetPropertyType.Strength.ToString(),
                    Value = 2
                }
            }
        };

        // Act & Assert
        character.Strength.Should().Be(16); // 14 base + 2 bonus
        character.StrModifier.Should().Be(3); // (16-10)/2 = 3
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(4, 2)]
    [InlineData(5, 3)]
    [InlineData(8, 3)]
    [InlineData(9, 4)]
    [InlineData(12, 4)]
    [InlineData(13, 5)]
    [InlineData(16, 5)]
    [InlineData(17, 6)]
    [InlineData(20, 6)]
    public void ProficiencyBonus_CalculatesCorrectly(int level, int expectedBonus)
    {
        // Arrange
        var character = new Character { Level = level };

        // Act & Assert
        character.ProficiencyBonus.Should().Be(expectedBonus);
    }

    [Fact]
    public void GetSkillBonus_WithoutProficiency_ReturnsOnlyStatModifier()
    {
        // Arrange
        var character = new Character
        {
            Wisdom = 14, // +2 modifier
            AcquiredFeatures = new List<Feature>()
        };

        // Act
        int bonus = character.GetSkillBonus("Perception", "Wisdom");

        // Assert
        bonus.Should().Be(2); // Solo el modificador de Wisdom, sin proficiency
    }

    [Fact]
    public void GetSkillBonus_WithProficiency_ReturnsStatModifierPlusProficiency()
    {
        // Arrange
        var character = new Character
        {
            Wisdom = 14, // +2 modifier
            Level = 5,   // +3 proficiency
            AcquiredFeatures = new List<Feature>
            {
                new() { TechnicalName = "SkillPerception" }
            }
        };

        // Act
        int bonus = character.GetSkillBonus("Perception", "Wisdom");

        // Assert
        bonus.Should().Be(5); // 2 (Wis) + 3 (Proficiency)
    }

    [Fact]
    public void Character_InitializesWithEmptyCollections()
    {
        // Arrange & Act
        var character = new Character();

        // Assert
        character.Stats.Should().BeEmpty();
        character.AcquiredFeatures.Should().BeEmpty();
        character.KnownSpells.Should().BeEmpty();
        character.CharacterModifiers.Should().BeEmpty();
        character.Inventory.Should().BeEmpty();
        character.SpellSlots.Should().BeEmpty();
        character.ActiveModifiers.Should().BeEmpty();
    }

    [Fact]
    public void Character_DefaultHp_Is10()
    {
        var character = new Character();
        character.MaxHp.Should().Be(10);
        character.CurrentHp.Should().Be(10);
    }

    [Fact]
    public void Character_DefaultLevel_Is1()
    {
        var character = new Character();
        character.Level.Should().Be(1);
        character.Experience.Should().Be(0);
    }
}