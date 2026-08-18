using RafeTale.Application.Services;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Exceptions;
using RafeTale.Domain.Helpers;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;
using System.Linq.Expressions;

namespace RafeTale.Tests.Application.Services;

public class SpellServiceSystemTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly SpellServiceSystem _sut;

    public SpellServiceSystemTests()
    {
        _sut = new SpellServiceSystem(_uow);
    }

    [Fact]
    public async Task CastSpellAsync_Cantrip_ReturnsTrueWithoutUsingSlot()
    {
        // Arrange
        var character = CreateCharacterWithSlots();
        _uow.Characters.GetByIdAsync(character.Id, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));

        // Act
        var result = await _sut.CastSpellAsync(character.Id, spellLevel: 0, slotLevelToUse: 0);

        // Assert
        result.Should().BeTrue();
        character.SpellSlots.First(s => s.Level == 1).UsedSlots.Should().Be(0);
        await _uow.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CastSpellAsync_SpellLevelHigherThanSlot_ReturnsFalse()
    {
        // Arrange
        var character = CreateCharacterWithSlots();
        _uow.Characters.GetByIdAsync(character.Id, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));

        // Act
        var result = await _sut.CastSpellAsync(character.Id, spellLevel: 3, slotLevelToUse: 2);

        // Assert
        result.Should().BeFalse();
        await _uow.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CastSpellAsync_NoSlotAvailable_ReturnsFalse()
    {
        // Arrange
        var character = CreateCharacterWithSlots();
        character.SpellSlots.First(s => s.Level == 1).UsedSlots = 4;
        _uow.Characters.GetByIdAsync(character.Id, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));

        // Act
        var result = await _sut.CastSpellAsync(character.Id, spellLevel: 1, slotLevelToUse: 1);

        // Assert
        result.Should().BeFalse();
        await _uow.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task CastSpellAsync_SlotAvailable_UsesSlotAndReturnsTrue()
    {
        // Arrange
        var character = CreateCharacterWithSlots();
        _uow.Characters.GetByIdAsync(character.Id, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));

        // Act
        var result = await _sut.CastSpellAsync(character.Id, spellLevel: 1, slotLevelToUse: 1);

        // Assert
        result.Should().BeTrue();
        character.SpellSlots.First(s => s.Level == 1).UsedSlots.Should().Be(1);
        await _uow.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task CastSpellAsync_CharacterNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _uow.Characters.GetByIdAsync(id, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(null));

        // Act
        var act = async () => await _sut.CastSpellAsync(id, spellLevel: 1, slotLevelToUse: 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RestRestoreSlotsAsync_ResetsUsedSlots()
    {
        // Arrange
        var character = CreateCharacterWithSlots();
        character.SpellSlots.First(s => s.Level == 1).UsedSlots = 3;
        character.SpellSlots.First(s => s.Level == 2).UsedSlots = 1;
        _uow.Characters.GetByIdAsync(character.Id).Returns(Task.FromResult<Character>(character));

        // Act
        await _sut.RestRestoreSlotsAsync(character.Id);

        // Assert
        character.SpellSlots.Should().OnlyContain(s => s.UsedSlots == 0);
        await _uow.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task RestRestoreSlotsAsync_CharacterNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _uow.Characters.GetByIdAsync(id).Returns((Character)null!);

        // Act
        var act = async () => await _sut.RestRestoreSlotsAsync(id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RecalculateMaxSlotsAsync_NoClassDef_ReturnsWithoutChanges()
    {
        // Arrange
        var character = CreateCharacter();
        character.ClassDef = null!;

        // Act
        await _sut.RecalculateMaxSlotsAsync(character);

        // Assert
        character.SpellSlots.Should().BeEmpty();
        await _uow.ClassLevelProgressions.DidNotReceive()
            .GetAllAsync(Arg.Any<Expression<Func<ClassLevelProgression, bool>>>(), Arg.Any<Action<IncludeAggregator<ClassLevelProgression>>>());
    }

    [Fact]
    public async Task RecalculateMaxSlotsAsync_NoProgression_ReturnsWithoutChanges()
    {
        // Arrange
        var character = CreateCharacter();
        _uow.ClassLevelProgressions.GetAllAsync(
                Arg.Any<Expression<Func<ClassLevelProgression, bool>>>(),
                Arg.Any<Action<IncludeAggregator<ClassLevelProgression>>>())
            .Returns(Task.FromResult<IEnumerable<ClassLevelProgression?>>([]));

        // Act
        await _sut.RecalculateMaxSlotsAsync(character);

        // Assert
        character.SpellSlots.Should().BeEmpty();
    }

    [Fact]
    public async Task RecalculateMaxSlotsAsync_NoSpellcastingTrait_ReturnsWithoutChanges()
    {
        // Arrange
        var character = CreateCharacter();
        var progression = CreateProgression(spellSlots: new int[9]);
        _uow.ClassLevelProgressions.GetAllAsync(
                Arg.Any<Expression<Func<ClassLevelProgression, bool>>>(),
                Arg.Any<Action<IncludeAggregator<ClassLevelProgression>>>())
            .Returns(Task.FromResult<IEnumerable<ClassLevelProgression?>>([progression]));

        // Act
        await _sut.RecalculateMaxSlotsAsync(character);

        // Assert
        character.SpellSlots.Should().BeEmpty();
    }

    [Fact]
    public async Task RecalculateMaxSlotsAsync_AddsNewSlots()
    {
        // Arrange
        var character = CreateCharacter();
        var progression = CreateProgression(spellSlots: [0, 3, 0, 0, 0, 0, 0, 0, 0]);
        _uow.ClassLevelProgressions.GetAllAsync(
                Arg.Any<Expression<Func<ClassLevelProgression, bool>>>(),
                Arg.Any<Action<IncludeAggregator<ClassLevelProgression>>>())
            .Returns(Task.FromResult<IEnumerable<ClassLevelProgression?>>([progression]));

        // Act
        await _sut.RecalculateMaxSlotsAsync(character);

        // Assert
        character.SpellSlots.Should().ContainSingle(s => s.Level == 2);
        character.SpellSlots.First(s => s.Level == 2).MaxSlots.Should().Be(3);
        _uow.Received().TrackNewSpellSlot(Arg.Any<CharacterSpellSlots>());
    }

    [Fact]
    public async Task RecalculateMaxSlotsAsync_UpdatesExistingSlots()
    {
        // Arrange
        var character = CreateCharacter();
        var existingSlot = new CharacterSpellSlots
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            Level = 2,
            MaxSlots = 1,
            UsedSlots = 5
        };
        character.SpellSlots.Add(existingSlot);

        var progression = CreateProgression(spellSlots: [0, 3, 0, 0, 0, 0, 0, 0, 0]);
        _uow.ClassLevelProgressions.GetAllAsync(
                Arg.Any<Expression<Func<ClassLevelProgression, bool>>>(),
                Arg.Any<Action<IncludeAggregator<ClassLevelProgression>>>())
            .Returns(Task.FromResult<IEnumerable<ClassLevelProgression?>>([progression]));
        _uow.SpellSlotExistsAsync(existingSlot.Id).Returns(Task.FromResult(true));

        // Act
        await _sut.RecalculateMaxSlotsAsync(character);

        // Assert
        existingSlot.MaxSlots.Should().Be(3);
        existingSlot.UsedSlots.Should().Be(3);
        _uow.DidNotReceive().TrackNewSpellSlot(Arg.Any<CharacterSpellSlots>());
    }

    [Fact]
    public async Task RecalculateMaxSlotsAsync_ExistingSlotNotInDatabase_CreatesNewSlot()
    {
        // Arrange
        var character = CreateCharacter();
        var existingSlot = new CharacterSpellSlots
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            Level = 2,
            MaxSlots = 1,
            UsedSlots = 1
        };
        character.SpellSlots.Add(existingSlot);

        var progression = CreateProgression(spellSlots: [0, 3, 0, 0, 0, 0, 0, 0, 0]);
        _uow.ClassLevelProgressions.GetAllAsync(
                Arg.Any<Expression<Func<ClassLevelProgression, bool>>>(),
                Arg.Any<Action<IncludeAggregator<ClassLevelProgression>>>())
            .Returns(Task.FromResult<IEnumerable<ClassLevelProgression?>>([progression]));
        _uow.SpellSlotExistsAsync(existingSlot.Id).Returns(Task.FromResult(false));

        // Act
        await _sut.RecalculateMaxSlotsAsync(character);

        // Assert
        character.SpellSlots.Should().ContainSingle(s => s.Level == 2);
        character.SpellSlots.First(s => s.Level == 2).MaxSlots.Should().Be(3);
        _uow.Received().TrackNewSpellSlot(Arg.Any<CharacterSpellSlots>());
    }

    [Fact]
    public async Task RecalculateMaxSlotsAsync_RemovesInactiveSlots()
    {
        // Arrange
        var character = CreateCharacter();
        character.SpellSlots.Add(new CharacterSpellSlots
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            Level = 5,
            MaxSlots = 2,
            UsedSlots = 0
        });

        var progression = CreateProgression(spellSlots: [0, 3, 0, 0, 0, 0, 0, 0, 0]);
        _uow.ClassLevelProgressions.GetAllAsync(
                Arg.Any<Expression<Func<ClassLevelProgression, bool>>>(),
                Arg.Any<Action<IncludeAggregator<ClassLevelProgression>>>())
            .Returns(Task.FromResult<IEnumerable<ClassLevelProgression?>>([progression]));

        // Act
        await _sut.RecalculateMaxSlotsAsync(character);

        // Assert
        character.SpellSlots.Should().NotContain(s => s.Level == 5);
    }

    // --- Helpers ---

    private static Character CreateCharacter()
    {
        var classDefId = Guid.NewGuid();
        return new Character
        {
            Id = Guid.NewGuid(),
            Name = "Gandalf",
            Level = 5,
            ClassDefId = classDefId,
            ClassDef = new ClassDefinition { Id = classDefId },
            SpellSlots = []
        };
    }

    private static Character CreateCharacterWithSlots()
    {
        var character = CreateCharacter();
        character.SpellSlots = [];
        {
            _ = new CharacterSpellSlots
            {
                Id = Guid.NewGuid(),
                CharacterId = character.Id,
                Level = 1,
                MaxSlots = 4,
                UsedSlots = 0
            };
            _ = new CharacterSpellSlots
            {
                Id = Guid.NewGuid(),
                CharacterId = character.Id,
                Level = 2,
                MaxSlots = 3,
                UsedSlots = 0
            };
        }
        ;
        return character;
    }

    private static ClassLevelProgression CreateProgression(int[] spellSlots)
    {
        return new ClassLevelProgression
        {
            Id = Guid.NewGuid(),
            Level = 5,
            ClassDefId = Guid.NewGuid(),
            Traits =
            [
                new ClassTrait
                {
                    Type = ResourceType.SpellSlots,
                    SpellSlots = spellSlots
                }
            ]
        };
    }
}
