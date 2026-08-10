using RafeTale.Application.Services;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;

namespace RafeTale.Tests.Application.Services;

public class LevelingServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly LevelingService _sut;

    public LevelingServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _sut = new LevelingService(_uow);
    }

    [Fact]
    public async Task AddExperienceAsync_When_CharacterNotFound_ReturnsFalse()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character>()));
        _uow.XpRules.GetXpThresholdsAsync().Returns(Task.FromResult(new Dictionary<int, int>()));

        // Act
        var result = await _sut.AddExperienceAsync(characterId, 100);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddExperienceAsync_When_XpBelowThreshold_DoesNotLevelUp()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 1, xp: 0);
        var thresholds = new Dictionary<int, int> { { 2, 300 } };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.XpRules.GetXpThresholdsAsync().Returns(Task.FromResult(thresholds));

        // Act
        var result = await _sut.AddExperienceAsync(characterId, 100);

        // Assert
        result.Should().BeFalse();
        character.Experience.Should().Be(100);
        character.Level.Should().Be(1);
        await _uow.Received().SaveChangesAsync();
        await _uow.Received().CommitAsync();
    }

    [Fact]
    public async Task AddExperienceAsync_When_XpReachesThreshold_LevelsUpAndAddsFeatures()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 1, xp: 250);
        var feature = new Feature
        {
            Id = featureId,
            TechnicalName = "Rage",
            Modifiers = new List<ModifierData>
            {
                new ModifierData { Type = ModifierType.AttributeBonus, Target = ASI.Strength.ToString(), Value = 2 }
            }
        };
        var progression = new ClassLevelProgression
        {
            Id = Guid.NewGuid(),
            ClassDefId = classDefId,
            Level = 2,
            Features = new List<Feature> { feature }
        };
        var thresholds = new Dictionary<int, int> { { 2, 300 } };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.XpRules.GetXpThresholdsAsync().Returns(Task.FromResult(thresholds));
        _uow.ClassLevelProgressions.GetProgressionsByClassAndLevelAsync(classDefId, 2).Returns(Task.FromResult<ClassLevelProgression?>(progression));

        // Act
        var result = await _sut.AddExperienceAsync(characterId, 50);

        // Assert
        result.Should().BeTrue();
        character.Level.Should().Be(2);
        character.Experience.Should().Be(300);
        character.AcquiredFeatures.Should().Contain(f => f.Id == featureId);
        character.CharacterModifiers.Should().Contain(m => m.Type == ModifierType.AttributeBonus && m.Target == ASI.Strength.ToString() && m.Value == 2 && m.CharacterId == characterId);
        await _uow.Received().SaveChangesAsync();
        await _uow.Received().CommitAsync();
    }

    [Fact]
    public async Task AddExperienceAsync_When_XpReachesMultipleThresholds_LevelsUpMultipleTimes()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 1, xp: 0);
        var thresholds = new Dictionary<int, int> { { 2, 300 }, { 3, 900 } };
        var progression2 = new ClassLevelProgression { Id = Guid.NewGuid(), ClassDefId = classDefId, Level = 2, Features = new List<Feature>() };
        var progression3 = new ClassLevelProgression { Id = Guid.NewGuid(), ClassDefId = classDefId, Level = 3, Features = new List<Feature>() };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.XpRules.GetXpThresholdsAsync().Returns(Task.FromResult(thresholds));
        _uow.ClassLevelProgressions.GetProgressionsByClassAndLevelAsync(classDefId, 2).Returns(Task.FromResult<ClassLevelProgression?>(progression2));
        _uow.ClassLevelProgressions.GetProgressionsByClassAndLevelAsync(classDefId, 3).Returns(Task.FromResult<ClassLevelProgression?>(progression3));

        // Act
        var result = await _sut.AddExperienceAsync(characterId, 900);

        // Assert
        result.Should().BeTrue();
        character.Level.Should().Be(3);
        character.Experience.Should().Be(900);
        await _uow.ClassLevelProgressions.Received().GetProgressionsByClassAndLevelAsync(classDefId, 2);
        await _uow.ClassLevelProgressions.Received().GetProgressionsByClassAndLevelAsync(classDefId, 3);
    }

    [Fact]
    public async Task AddExperienceAsync_When_FeatureAlreadyAcquired_DoesNotDuplicate()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        var feature = new Feature
        {
            Id = featureId,
            TechnicalName = "ExtraAttack",
            Modifiers = new List<ModifierData>()
        };
        var character = CreateCharacter(characterId, classDefId, level: 4, xp: 2500);
        character.AcquiredFeatures.Add(feature);
        var progression = new ClassLevelProgression
        {
            Id = Guid.NewGuid(),
            ClassDefId = classDefId,
            Level = 5,
            Features = new List<Feature> { feature }
        };
        var thresholds = new Dictionary<int, int> { { 5, 3000 } };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.XpRules.GetXpThresholdsAsync().Returns(Task.FromResult(thresholds));
        _uow.ClassLevelProgressions.GetProgressionsByClassAndLevelAsync(classDefId, 5).Returns(Task.FromResult<ClassLevelProgression?>(progression));

        // Act
        var result = await _sut.AddExperienceAsync(characterId, 500);

        // Assert
        result.Should().BeTrue();
        character.Level.Should().Be(5);
        character.AcquiredFeatures.Count.Should().Be(1);
        character.CharacterModifiers.Should().BeEmpty();
    }

    [Fact]
    public async Task AddExperienceAsync_When_RepositoryThrows_RollsBackAndThrows()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 1, xp: 0);
        var thresholds = new Dictionary<int, int> { { 2, 300 } };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.XpRules.GetXpThresholdsAsync().Returns(Task.FromResult(thresholds));
        _uow.ClassLevelProgressions.GetProgressionsByClassAndLevelAsync(classDefId, 2).Returns(Task.FromResult<ClassLevelProgression?>(null));
        _uow.When(x => x.SaveChangesAsync()).Do(_ => throw new InvalidOperationException("DB failure"));

        // Act
        var act = async () => await _sut.AddExperienceAsync(characterId, 300);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        await _uow.Received().RollbackAsync();
    }

    [Fact]
    public async Task CommitLevelUpAsync_When_CharacterNotFound_ReturnsFalse()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character>()));

        // Act
        var result = await _sut.CommitLevelUpAsync(characterId, 5, new List<CharacterModifier>(), new List<Guid>(), new List<Guid>());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CommitLevelUpAsync_When_Valid_AppliesModifiers()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 2, xp: 300);
        var modifiers = new List<CharacterModifier>
        {
            new CharacterModifier { Id = Guid.NewGuid(), Type = ModifierType.AttributeBonus, Target = ASI.Constitution.ToString(), Value = 1, CharacterId = characterId }
        };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));

        // Act
        var result = await _sut.CommitLevelUpAsync(characterId, 5, modifiers, new List<Guid>(), new List<Guid>());

        // Assert
        result.Should().BeTrue();
        character.CharacterModifiers.Should().Contain(modifiers.Single());
        await _uow.Received().SaveChangesAsync();
        await _uow.Received().CommitAsync();
    }

    [Fact]
    public async Task CommitLevelUpAsync_When_Valid_AppliesFeatsAndTheirModifiers()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var featId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 3, xp: 900);
        var feat = new Feat
        {
            Id = featId,
            TechnicalName = "Tough",
            Modifiers = new List<ModifierData>
            {
                new ModifierData { Type = ModifierType.HpBonus, Target = TargetPropertyType.MaxHp.ToString(), Value = 2 }
            }
        };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.Feats.GetAllAsync().Returns(Task.FromResult<IEnumerable<Feat?>>(new List<Feat?> { feat }));

        // Act
        var result = await _sut.CommitLevelUpAsync(characterId, 0, new List<CharacterModifier>(), new List<Guid> { featId }, new List<Guid>());

        // Assert
        result.Should().BeTrue();
        character.AcquiredFeats.Should().Contain(f => f.Id == featId);
        character.CharacterModifiers.Should().Contain(m => m.Source == "Dote: Tough" && m.Type == ModifierType.HpBonus && m.Value == 2);
    }

    [Fact]
    public async Task CommitLevelUpAsync_When_Valid_AppliesSpells()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var spellId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 2, xp: 300);
        var spell = new Spell { Id = spellId, TechnicalName = "Fireball" };

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.Spells.GetAllAsync().Returns(Task.FromResult<IEnumerable<Spell?>>(new List<Spell?> { spell }));

        // Act
        var result = await _sut.CommitLevelUpAsync(characterId, 0, new List<CharacterModifier>(), new List<Guid>(), new List<Guid> { spellId });

        // Assert
        result.Should().BeTrue();
        character.KnownSpells.Should().Contain(s => s.Id == spellId);
    }

    [Fact]
    public async Task CommitLevelUpAsync_When_FeatAlreadyAcquired_DoesNotDuplicate()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var featId = Guid.NewGuid();
        var feat = new Feat { Id = featId, TechnicalName = "Tough", Modifiers = new List<ModifierData>() };
        var character = CreateCharacter(characterId, classDefId, level: 4, xp: 2700);
        character.AcquiredFeats.Add(feat);

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.Feats.GetAllAsync().Returns(Task.FromResult<IEnumerable<Feat?>>(new List<Feat?> { feat }));

        // Act
        var result = await _sut.CommitLevelUpAsync(characterId, 0, new List<CharacterModifier>(), new List<Guid> { featId }, new List<Guid>());

        // Assert
        result.Should().BeTrue();
        character.AcquiredFeats.Count.Should().Be(1);
        character.CharacterModifiers.Should().BeEmpty();
    }

    [Fact]
    public async Task CommitLevelUpAsync_When_SpellAlreadyKnown_DoesNotDuplicate()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var spellId = Guid.NewGuid();
        var spell = new Spell { Id = spellId, TechnicalName = "Fireball" };
        var character = CreateCharacter(characterId, classDefId, level: 2, xp: 300);
        character.KnownSpells.Add(spell);

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.Spells.GetAllAsync().Returns(Task.FromResult<IEnumerable<Spell?>>(new List<Spell?> { spell }));

        // Act
        var result = await _sut.CommitLevelUpAsync(characterId, 0, new List<CharacterModifier>(), new List<Guid>(), new List<Guid> { spellId });

        // Assert
        result.Should().BeTrue();
        character.KnownSpells.Count.Should().Be(1);
    }

    [Fact]
    public async Task CommitLevelUpAsync_When_RepositoryThrows_RollsBackAndThrows()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var classDefId = Guid.NewGuid();
        var character = CreateCharacter(characterId, classDefId, level: 2, xp: 300);

        _uow.Characters.GetAllWithDetailsAsync().Returns(Task.FromResult<IEnumerable<Character>>(new List<Character> { character }));
        _uow.When(x => x.SaveChangesAsync()).Do(_ => throw new InvalidOperationException("DB failure"));

        // Act
        var act = async () => await _sut.CommitLevelUpAsync(characterId, 5, new List<CharacterModifier>(), new List<Guid>(), new List<Guid>());

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        await _uow.Received().RollbackAsync();
    }

    private static Character CreateCharacter(Guid id, Guid classDefId, int level, int xp = 0)
    {
        return new Character
        {
            Id = id,
            Name = "Test Hero",
            ClassDefId = classDefId,
            Level = level,
            Experience = xp,
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
            AcquiredFeatures = new List<Feature>(),
            AcquiredFeats = new List<Feat>(),
            KnownSpells = new List<Spell>(),
            CharacterModifiers = new List<CharacterModifier>()
        };
    }
}
