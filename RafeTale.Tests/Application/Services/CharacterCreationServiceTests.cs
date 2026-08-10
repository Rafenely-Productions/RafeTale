using RafeTale.Application.Services;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Exceptions;
using RafeTale.Domain.Interfaces;

namespace RafeTale.Tests.Application.Services;

public class CharacterCreationServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly CharacterCreationService _sut;

    public CharacterCreationServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _sut = new CharacterCreationService();
    }

    [Fact]
    public void Reset_Should_Clear_All_Selections_And_Stats()
    {
        // Arrange
        _sut.SelectedRaceId = Guid.NewGuid();
        _sut.SelectedClassId = Guid.NewGuid();
        _sut.SelectedBackgroundId = Guid.NewGuid();
        _sut.Name = "Test";
        _sut.History = "History";
        _sut.BaseStats[ASI.Strength] = 16;
        _sut.BonusStats[ASI.Dexterity] = 2;

        // Act
        _sut.Reset();

        // Assert
        _sut.SelectedRaceId.Should().BeNull();
        _sut.SelectedClassId.Should().BeNull();
        _sut.SelectedBackgroundId.Should().BeNull();
        _sut.Name.Should().BeEmpty();
        _sut.History.Should().BeEmpty();
        _sut.BaseStats.Values.Should().AllBeEquivalentTo(10);
        _sut.BonusStats.Values.Should().AllBeEquivalentTo(0);
    }

    [Theory]
    [InlineData(10, 0, 8)]   // con 10 -> mod 0 -> d8 = 8
    [InlineData(14, 0, 10)]  // con 14 -> mod +2 -> d8 + 2 = 10
    [InlineData(8, 0, 7)]    // con 8 -> mod -1 -> d8 - 1 = 7
    [InlineData(10, 2, 9)]   // con 12 -> mod +1 -> d8 + 1 = 9
    public async Task CreateAndSaveCharacterAsync_When_Valid_Should_Create_Character_With_Correct_Hp(int baseCon, int bonusCon, int expectedHp)
    {
        // Arrange
        var raceId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();

        _sut.SelectedRaceId = raceId;
        _sut.SelectedClassId = classId;
        _sut.SelectedBackgroundId = backgroundId;
        _sut.Name = "Aragorn";
        _sut.History = "A hero";
        _sut.BaseStats[ASI.Constitution] = baseCon;
        _sut.BonusStats[ASI.Constitution] = bonusCon;

        _uow.Races.GetByIdAsync(raceId).Returns(Task.FromResult<Race?>(new Race { Id = raceId }));
        _uow.ClassDefinitions.GetByIdAsync(classId).Returns(Task.FromResult<ClassDefinition?>(new ClassDefinition { Id = classId, HitDie = "d8" }));
        _uow.Backgrounds.GetByIdAsync(backgroundId).Returns(Task.FromResult<Background?>(new Background { Id = backgroundId }));

        // Act
        var result = await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Aragorn");
        result.History.Should().Be("A hero");
        result.RaceId.Should().Be(raceId);
        result.ClassDefId.Should().Be(classId);
        result.BackgroundId.Should().Be(backgroundId);
        result.Level.Should().Be(1);
        result.Experience.Should().Be(0);
        result.MaxHp.Should().Be(expectedHp);
        result.CurrentHp.Should().Be(expectedHp);
        await _uow.Characters.Received().AddAsync(result);
        await _uow.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_Valid_Should_Add_BonusStats_As_Modifiers()
    {
        // Arrange
        var raceId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();

        _sut.SelectedRaceId = raceId;
        _sut.SelectedClassId = classId;
        _sut.SelectedBackgroundId = backgroundId;
        _sut.BonusStats[ASI.Strength] = 2;
        _sut.BonusStats[ASI.Charisma] = 1;

        _uow.Races.GetByIdAsync(raceId).Returns(Task.FromResult<Race?>(new Race { Id = raceId }));
        _uow.ClassDefinitions.GetByIdAsync(classId).Returns(Task.FromResult<ClassDefinition?>(new ClassDefinition { Id = classId, HitDie = "d10" }));
        _uow.Backgrounds.GetByIdAsync(backgroundId).Returns(Task.FromResult<Background?>(new Background { Id = backgroundId }));

        // Act
        var result = await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        result.CharacterModifiers.Should().HaveCount(2);
        result.CharacterModifiers.Should().Contain(m => m.Type == ModifierType.AttributeBonus && m.Target == ASI.Strength.ToString() && m.Value == 2);
        result.CharacterModifiers.Should().Contain(m => m.Type == ModifierType.AttributeBonus && m.Target == ASI.Charisma.ToString() && m.Value == 1);
        result.CharacterModifiers.Should().OnlyContain(m => m.CharacterId == result.Id);
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_Valid_Should_Reset_Service_State_After_Saving()
    {
        // Arrange
        var raceId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();

        _sut.SelectedRaceId = raceId;
        _sut.SelectedClassId = classId;
        _sut.SelectedBackgroundId = backgroundId;
        _sut.Name = "Gandalf";

        _uow.Races.GetByIdAsync(raceId).Returns(Task.FromResult<Race?>(new Race { Id = raceId }));
        _uow.ClassDefinitions.GetByIdAsync(classId).Returns(Task.FromResult<ClassDefinition?>(new ClassDefinition { Id = classId, HitDie = "d6" }));
        _uow.Backgrounds.GetByIdAsync(backgroundId).Returns(Task.FromResult<Background?>(new Background { Id = backgroundId }));

        // Act
        await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        _sut.SelectedRaceId.Should().BeNull();
        _sut.SelectedClassId.Should().BeNull();
        _sut.SelectedBackgroundId.Should().BeNull();
        _sut.Name.Should().BeEmpty();
        _sut.BaseStats.Values.Should().AllBeEquivalentTo(10);
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_MissingRace_Should_Throw_DomainValidationException()
    {
        // Arrange
        _sut.SelectedClassId = Guid.NewGuid();
        _sut.SelectedBackgroundId = Guid.NewGuid();

        // Act
        var act = async () => await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>();
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_MissingClass_Should_Throw_DomainValidationException()
    {
        // Arrange
        _sut.SelectedRaceId = Guid.NewGuid();
        _sut.SelectedBackgroundId = Guid.NewGuid();

        // Act
        var act = async () => await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>();
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_MissingBackground_Should_Throw_DomainValidationException()
    {
        // Arrange
        _sut.SelectedRaceId = Guid.NewGuid();
        _sut.SelectedClassId = Guid.NewGuid();

        // Act
        var act = async () => await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>();
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_RaceNotFound_Should_Throw_NotFoundException()
    {
        // Arrange
        var raceId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();

        _sut.SelectedRaceId = raceId;
        _sut.SelectedClassId = classId;
        _sut.SelectedBackgroundId = backgroundId;

        _uow.Races.GetByIdAsync(raceId).Returns(Task.FromResult<Race?>(null));

        // Act
        var act = async () => await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_ClassNotFound_Should_Throw_NotFoundException()
    {
        // Arrange
        var raceId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();

        _sut.SelectedRaceId = raceId;
        _sut.SelectedClassId = classId;
        _sut.SelectedBackgroundId = backgroundId;

        _uow.Races.GetByIdAsync(raceId).Returns(Task.FromResult<Race?>(new Race { Id = raceId }));
        _uow.ClassDefinitions.GetByIdAsync(classId).Returns(Task.FromResult<ClassDefinition?>(null));

        // Act
        var act = async () => await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAndSaveCharacterAsync_When_BackgroundNotFound_Should_Throw_NotFoundException()
    {
        // Arrange
        var raceId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();

        _sut.SelectedRaceId = raceId;
        _sut.SelectedClassId = classId;
        _sut.SelectedBackgroundId = backgroundId;

        _uow.Races.GetByIdAsync(raceId).Returns(Task.FromResult<Race?>(new Race { Id = raceId }));
        _uow.ClassDefinitions.GetByIdAsync(classId).Returns(Task.FromResult<ClassDefinition?>(new ClassDefinition { Id = classId, HitDie = "d8" }));
        _uow.Backgrounds.GetByIdAsync(backgroundId).Returns(Task.FromResult<Background?>(null));

        // Act
        var act = async () => await _sut.CreateAndSaveCharacterAsync(_uow);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}