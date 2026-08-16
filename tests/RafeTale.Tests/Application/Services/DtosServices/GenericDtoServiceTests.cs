using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Interfaces.DtosInterfaces;
using RafeTale.Application.Services.DtosServices;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Helpers;
using RafeTale.Domain.Interfaces;
using System.Linq.Expressions;

namespace RafeTale.Tests.Application.Services.DtosServices;

public abstract class GenericDtoServiceTests<TDto, TEntity, TService>
    where TDto : class
    where TEntity : class
    where TService : class, IService<TDto, TEntity>
{
    protected readonly IUnitOfWork Uow = Substitute.For<IUnitOfWork>();
    protected readonly ILocalizationService Loc = Substitute.For<ILocalizationService>();
    protected readonly IService<TDto, TEntity> Sut;

    protected GenericDtoServiceTests()
    {
        Sut = CreateService(Uow, Loc);
    }

    protected abstract IService<TDto, TEntity> CreateService(IUnitOfWork uow, ILocalizationService loc);
    protected abstract TEntity CreateEntity();
    protected abstract Guid GetEntityId(TEntity entity);
    protected abstract LocEntity EntityType { get; }
    protected abstract Dictionary<LocProperty, Dictionary<Guid, string>> CreateLocalizedWords(Guid entityId);
    protected abstract void SetupRepositoryGetAll(IEnumerable<TEntity> entities);
    protected abstract void SetupRepositoryGetById(Guid id, TEntity? entity);
    protected abstract void SetupRepositoryGetAllThrows(Exception exception);
    protected abstract void SetupRepositoryGetByIdThrows(Guid id, Exception exception);
    protected abstract void AssertMappedDto(TDto dto, TEntity entity);

    protected virtual void SetupLocalizationStrings(Guid entityId) { }

    [Fact]
    public async Task GetAllAsync_WithEntities_ReturnsMappedDtos()
    {
        // Arrange
        var entity = CreateEntity();
        var entities = new List<TEntity> { entity };
        var localizedWords = CreateLocalizedWords(GetEntityId(entity));

        SetupRepositoryGetAll(entities);
        Loc.GetAllAsync(EntityType, Arg.Any<LocProperty[]>()).Returns(Task.FromResult(localizedWords));

        // Act
        var result = await Sut.GetAllAsync(null, null);

        // Assert
        result.Should().HaveCount(1);
        AssertMappedDto(result[0], entity);
    }

    [Fact]
    public async Task GetAllAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var entities = new List<TEntity>();
        var localizedWords = new Dictionary<LocProperty, Dictionary<Guid, string>>();

        SetupRepositoryGetAll(entities);
        Loc.GetAllAsync(EntityType, Arg.Any<LocProperty[]>()).Returns(Task.FromResult(localizedWords));

        // Act
        var result = await Sut.GetAllAsync(null, null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        SetupRepositoryGetAllThrows(new Exception("DB failure"));

        // Act
        var act = async () => await Sut.GetAllAsync(null, null);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("DB failure");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingEntity_ReturnsMappedDto()
    {
        // Arrange
        var entity = CreateEntity();
        var id = GetEntityId(entity);

        SetupRepositoryGetById(id, entity);
        SetupLocalizationStrings(id);

        // Act
        var result = await Sut.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        AssertMappedDto(result, entity);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEntity_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        SetupRepositoryGetById(id, null);

        // Act
        var result = await Sut.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var id = Guid.NewGuid();
        SetupRepositoryGetByIdThrows(id, new Exception("DB failure"));

        // Act
        var act = async () => await Sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("DB failure");
    }

    [Fact]
    public void ArmDto_WithLocalizedWords_ReturnsMappedDto()
    {
        // Arrange
        var entity = CreateEntity();
        var localizedWords = CreateLocalizedWords(GetEntityId(entity));

        // Act
        var result = Sut.ArmDto(entity, localizedWords);

        // Assert
        result.Should().NotBeNull();
        AssertMappedDto(result, entity);
    }

    [Fact]
    public async Task ArmDto_Async_ReturnsMappedDto()
    {
        // Arrange
        var entity = CreateEntity();
        var id = GetEntityId(entity);
        SetupLocalizationStrings(id);

        // Act
        var result = await Sut.ArmDto(entity);

        // Assert
        result.Should().NotBeNull();
        AssertMappedDto(result, entity);
    }
}

public class SpellServiceGenericDtoTests : GenericDtoServiceTests<SpellDto, Spell, SpellService>
{
    protected override IService<SpellDto, Spell> CreateService(IUnitOfWork uow, ILocalizationService loc)
        => new SpellService(uow, loc);

    protected override Spell CreateEntity()
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

    protected override Guid GetEntityId(Spell entity) => entity.Id;

    protected override LocEntity EntityType => LocEntity.Spell;

    protected override Dictionary<LocProperty, Dictionary<Guid, string>> CreateLocalizedWords(Guid entityId)
    {
        return new Dictionary<LocProperty, Dictionary<Guid, string>>
        {
            [LocProperty.Name] = new Dictionary<Guid, string> { [entityId] = "Fireball" },
            [LocProperty.Description] = new Dictionary<Guid, string> { [entityId] = "A fiery explosion" },
            [LocProperty.MaterialComponentDescription] = new Dictionary<Guid, string> { [entityId] = "A tiny ball of bat guano" }
        };
    }

    protected override void SetupRepositoryGetAll(IEnumerable<Spell> entities)
    {
        Uow.Spells.GetAllAsync(Arg.Any<Expression<Func<Spell, bool>>>(), Arg.Any<Action<IncludeAggregator<Spell>>>())
            .Returns(Task.FromResult<IEnumerable<Spell?>>(entities));
    }

    protected override void SetupRepositoryGetById(Guid id, Spell? entity)
    {
        Uow.Spells.GetByIdAsync(id).Returns(Task.FromResult<Spell?>(entity));
    }

    protected override void SetupRepositoryGetAllThrows(Exception exception)
    {
        Uow.Spells.GetAllAsync(Arg.Any<Expression<Func<Spell, bool>>>(), Arg.Any<Action<IncludeAggregator<Spell>>>())
            .Returns(Task.FromException<IEnumerable<Spell?>>(exception));
    }

    protected override void SetupRepositoryGetByIdThrows(Guid id, Exception exception)
    {
        Uow.Spells.GetByIdAsync(id).Returns(Task.FromException<Spell?>(exception));
    }

    protected override void SetupLocalizationStrings(Guid entityId)
    {
        Loc.GetStringAsync(entityId, LocProperty.Name).Returns(Task.FromResult("Fireball"));
        Loc.GetStringAsync(entityId, LocProperty.Description).Returns(Task.FromResult("A fiery explosion"));
        Loc.GetStringAsync(entityId, LocProperty.MaterialComponentDescription).Returns(Task.FromResult("A tiny ball of bat guano"));
    }

    protected override void AssertMappedDto(SpellDto dto, Spell entity)
    {
        dto.Id.Should().Be(entity.Id);
        dto.TechnicalName.Should().Be("Fireball");
    }
}