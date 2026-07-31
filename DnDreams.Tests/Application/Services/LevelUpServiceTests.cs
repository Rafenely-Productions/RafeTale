using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Application.Services;
using DnDreams.Application.Services.DtosServices;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Exceptions;
using DnDreams.Domain.Helpers;
using DnDreams.Domain.Interfaces;
using DnDreams.Domain.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;

namespace DnDreams.Tests.Application.Services;

public class LevelUpServiceTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IService<CharacterDto, Character> _characterService = Substitute.For<IService<CharacterDto, Character>>();
    private readonly ISpellServiceSystem _spellService = Substitute.For<ISpellServiceSystem>();
    private readonly ILogger<LevelUpService> _logger = Substitute.For<ILogger<LevelUpService>>();
    private readonly LevelUpService _sut;
    public LevelUpServiceTests()
    {
        _sut = new LevelUpService(_uow, _characterService, _spellService, _logger);
    }

    [Fact]
    public async Task PrepareLevelUpAsync_CharacterNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        _uow.Characters.GetByIdAsync(characterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(null));

        // Act
        var act = async () => await _sut.PrepareLevelUpAsync(characterId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task PrepareLevelUpAsync_ValidCharacter_ReturnsDraftWithNextLevel()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 1);
        var classDef = CreateClassDefinition(classDefId, hitDie: "d8");

        _uow.Characters.GetByIdAsync(characterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));
        _uow.ClassDefinitions.GetByIdAsync(classDefId, Arg.Any<Action<IncludeAggregator<ClassDefinition>>>())
            .Returns(Task.FromResult<ClassDefinition?>(classDef));

        // Act
        var result = await _sut.PrepareLevelUpAsync(characterId);

        // Assert
        result.Should().NotBeNull();
        result.TargetLevel.Should().Be(2);
        result.GivesFeat.Should().BeFalse();
        result.CharacterId.Should().Be(characterId);
    }

    [Theory]
    [InlineData(3, 4, true)]  // Nivel 4 da feat
    [InlineData(7, 8, true)]  // Nivel 8 da feat
    [InlineData(1, 2, false)] // Nivel 2 no da feat
    [InlineData(5, 6, false)] // Nivel 6 no da feat
    public async Task PrepareLevelUpAsync_VariousLevels_ReturnsCorrectFeatFlag(int currentLevel, int expectedTargetLevel, bool expectedGivesFeat)
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: currentLevel);
        var classDef = CreateClassDefinition(classDefId, hitDie: "d8");

        _uow.Characters.GetByIdAsync(characterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));
        _uow.ClassDefinitions.GetByIdAsync(classDefId, Arg.Any<Action<IncludeAggregator<ClassDefinition>>>())
            .Returns(Task.FromResult<ClassDefinition?>(classDef));

        // Act
        var result = await _sut.PrepareLevelUpAsync(characterId);

        // Assert
        result.TargetLevel.Should().Be(expectedTargetLevel);
        result.GivesFeat.Should().Be(expectedGivesFeat);
    }

    [Fact]
    public async Task PrepareClaimDraftAsync_ValidCharacter_ReturnsCorrectDraft()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 5);
        var classDef = CreateClassDefinition(classDefId, hitDie: "d10");
        character.ClassDef = classDef;

        _uow.Characters.GetByIdAsync(characterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));
        _uow.ClassDefinitions.GetByIdAsync(classDefId, Arg.Any<Action<IncludeAggregator<ClassDefinition>>>())
            .Returns(Task.FromResult<ClassDefinition?>(classDef));

        // Act
        var result = await _sut.PrepareClaimDraftAsync(characterId);

        // Assert
        result.Should().NotBeNull();
        result.TargetLevel.Should().Be(5); // No sube de nivel en claim
        result.HpGain.Should().Be(0);
    }

    [Fact]
    public async Task CommitLevelUpAsync_CharacterNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var draft = new LevelUpDraft { CharacterId = Guid.NewGuid() };
        _uow.Characters.GetByIdAsync(draft.CharacterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(null));

        // Act
        var act = async () => await _sut.CommitLevelUpAsync(draft);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CommitLevelUpAsync_ValidDraft_UpdatesCharacterLevelAndHp()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 1);
        var classDef = CreateClassDefinition(classDefId, hitDie: "d8");

        _uow.Characters.GetByIdAsync(characterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));
        _uow.ClassDefinitions.GetByIdAsync(classDefId, Arg.Any<Action<IncludeAggregator<ClassDefinition>>>())
            .Returns(Task.FromResult<ClassDefinition?>(classDef));
        _uow.SaveChangesAsync().Returns(Task.FromResult(1));
        _characterService.ArmDto(Arg.Any<Character>()).Returns(new CharacterDto { Id = characterId, Name = "Hero" });

        var draft = new LevelUpDraft
        {
            CharacterId = characterId,
            TargetLevel = 2,
            HpGain = 6,
            GivesFeat = false,
            SpellBudget = new SpellBudget()
        };

        // Act
        var result = await _sut.CommitLevelUpAsync(draft);

        // Assert
        character.Level.Should().Be(2);
        character.MaxHp.Should().Be(16); // 10 base + 6 gain
        character.CurrentHp.Should().Be(16);
        await _uow.Received().SaveChangesAsync();
    }

    [Fact]
    public async Task CommitLevelUpAsync_WithFeatSelection_AddsFeatToCharacter()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var featId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 3);
        var classDef = CreateClassDefinition(classDefId, hitDie: "d8");
        var feat = new Feat { Id = featId, TechnicalName = "Tough" };

        _uow.Characters.GetByIdAsync(characterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));
        _uow.ClassDefinitions.GetByIdAsync(classDefId, Arg.Any<Action<IncludeAggregator<ClassDefinition>>>())
            .Returns(Task.FromResult<ClassDefinition?>(classDef));
        _uow.Feats.GetByIdAsync(featId).Returns(Task.FromResult<Feat?>(feat));
        _uow.SaveChangesAsync().Returns(Task.FromResult(1));
        _characterService.ArmDto(Arg.Any<Character>()).Returns(new CharacterDto { Id = characterId, Name = "Hero" });

        var draft = new LevelUpDraft
        {
            CharacterId = characterId,
            TargetLevel = 4,
            HpGain = 5,
            GivesFeat = true,
            SelectedFeatId = featId,
            SpellBudget = new SpellBudget()
        };

        // Act
        await _sut.CommitLevelUpAsync(draft);

        // Assert
        character.AcquiredFeats.Should().Contain(f => f.Id == featId);
    }

    [Fact]
    public async Task AuditCharacterAsync_ValidCharacter_ReturnsCorrectPendingCounts()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 1);
        var classDef = CreateClassDefinition(classDefId, hitDie: "d8");
        character.ClassDef = classDef;

        _uow.Characters.GetByIdAsync(characterId, Arg.Any<Action<IncludeAggregator<Character>>>())
            .Returns(Task.FromResult<Character?>(character));

        // Act
        var result = await _sut.AuditCharacterAsync(characterId);

        // Assert
        result.Should().NotBeNull();
        result.PendingFeats.Should().Be(0);
        result.PendingSpells.Should().Be(0);
    }

    // --- Helpers ---

    private static Character CreateCharacter(Guid id, Guid classDefId, int level)
    {
        return new Character
        {
            Id = id,
            Name = "Test Hero",
            ClassDefId = classDefId,
            Level = level,
            MaxHp = 10,
            CurrentHp = 10,
            Stats = new Dictionary<string, int>
            {
                { TargetPropertyType.Strength.ToString(), 10 },
                { TargetPropertyType.Dexterity.ToString(), 10 },
                { TargetPropertyType.Constitution.ToString(), 10 },
                { TargetPropertyType.Intelligence.ToString(), 10 },
                { TargetPropertyType.Wisdom.ToString(), 10 },
                { TargetPropertyType.Charisma.ToString(), 10 }
            },
            KnownSpells = new List<Spell>(),
            AcquiredFeatures = new List<Feature>(),
            AcquiredFeats = new List<Feat>(),
            CharacterModifiers = new List<CharacterModifier>(),
            SpellSlots = new List<CharacterSpellSlots>()
        };
    }

    private static ClassDefinition CreateClassDefinition(Guid id, string hitDie)
    {
        return new ClassDefinition
        {
            Id = id,
            TechnicalName = "TestClass",
            HitDie = hitDie,
            Progressions = new List<ClassLevelProgression>()
        };
    }
}
