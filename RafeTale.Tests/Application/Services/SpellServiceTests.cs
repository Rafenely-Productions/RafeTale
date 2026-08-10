using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Services.DtosServices;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Helpers;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Interfaces.IRepositories;
using System.Linq.Expressions;

namespace RafeTale.Tests.Application.Services;

public class SpellServiceTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ILocalizationService _loc = Substitute.For<ILocalizationService>();
    private readonly SpellService _sut;

    public SpellServiceTests()
    {
        _sut = new SpellService(_uow, _loc);
    }

    [Fact]
    public async Task GetAllAsync_WithSpells_ReturnsMappedDtos()
    {
        // Arrange
        var spell = CreateSpell();
        var spells = new List<Spell> { spell };
        var localizedWords = CreateLocalizedWords(spell.Id);

        _uow.Spells.GetAllAsync(Arg.Any<Expression<Func<Spell, bool>>>(), Arg.Any<Action<IncludeAggregator<Spell>>>())
            .Returns(Task.FromResult<IEnumerable<Spell?>>(spells));
        _loc.GetAllAsync(LocEntity.Spell, Arg.Any<LocProperty[]>())
            .Returns(Task.FromResult(localizedWords));

        // Act
        var result = await _sut.GetAllAsync(null, null);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(spell.Id);
        result[0].Name.Should().Be("Fireball");
    }

    [Fact]
    public async Task GetAllAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var spells = new List<Spell>();
        var localizedWords = new Dictionary<LocProperty, Dictionary<Guid, string>>();

        _uow.Spells.GetAllAsync(Arg.Any<Expression<Func<Spell, bool>>>(), Arg.Any<Action<IncludeAggregator<Spell>>>())
            .Returns(Task.FromResult<IEnumerable<Spell?>>(spells));
        _loc.GetAllAsync(LocEntity.Spell, Arg.Any<LocProperty[]>())
            .Returns(Task.FromResult(localizedWords));

        // Act
        var result = await _sut.GetAllAsync(null, null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        _uow.Spells.GetAllAsync(Arg.Any<Expression<Func<Spell, bool>>>(), Arg.Any<Action<IncludeAggregator<Spell>>>())
            .Returns(Task.FromException<IEnumerable<Spell?>>(new Exception("DB failure")));

        // Act
        var act = async () => await _sut.GetAllAsync(null, null);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("DB failure");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSpell_ReturnsMappedDto()
    {
        // Arrange
        var spell = CreateSpell();
        _uow.Spells.GetByIdAsync(spell.Id).Returns(Task.FromResult<Spell?>(spell));
        _loc.GetStringAsync(spell.Id, LocProperty.Name).Returns(Task.FromResult("Fireball"));
        _loc.GetStringAsync(spell.Id, LocProperty.Description).Returns(Task.FromResult("A fiery explosion"));
        _loc.GetStringAsync(spell.Id, LocProperty.MaterialComponentDescription).Returns(Task.FromResult("A tiny ball of bat guano"));

        // Act
        var result = await _sut.GetByIdAsync(spell.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(spell.Id);
        result.Name.Should().Be("Fireball");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingSpell_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _uow.Spells.GetByIdAsync(id).Returns(Task.FromResult<Spell?>(null));

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _uow.Spells.GetByIdAsync(id)
            .Returns(Task.FromException<Spell?>(new Exception("DB failure")));

        // Act
        var act = async () => await _sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("DB failure");
    }

    // --- Helpers ---

    private static Spell CreateSpell()
    {
        return new Spell
        {
            Id = Guid.NewGuid(),
            TechnicalName = "Fireball",
            Level = SpellLevel.Level3,
            School = SchoolOfMagicEnum.Evocation,
            CastingTime = CastingTime.Action,
            Range = SpellRange.Ranged,
            RangeDistance = "150 feet",
            Components = new List<SpellComponent> { SpellComponent.V, SpellComponent.S, SpellComponent.M },
            Duration = new List<SpellDuration> { SpellDuration.Instantaneous },
            Concentration = SpellConcentration.No,
            Ritual = false,
            ClassesTechnicalNames = new List<string> { "Wizard", "Sorcerer" }
        };
    }

    private static Dictionary<LocProperty, Dictionary<Guid, string>> CreateLocalizedWords(Guid spellId)
    {
        return new Dictionary<LocProperty, Dictionary<Guid, string>>
        {
            [LocProperty.Name] = new Dictionary<Guid, string> { [spellId] = "Fireball" },
            [LocProperty.Description] = new Dictionary<Guid, string> { [spellId] = "A fiery explosion" },
            [LocProperty.MaterialComponentDescription] = new Dictionary<Guid, string> { [spellId] = "A tiny ball of bat guano" }
        };
    }
}
