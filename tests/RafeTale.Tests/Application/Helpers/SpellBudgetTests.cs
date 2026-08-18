using RafeTale.Application.DTOs;
using RafeTale.Application.Services;
using RafeTale.Domain.Enums;
using Xunit; // Asegúrate de tener este using

namespace RafeTale.Tests.Application.Helpers;

public class SpellBudgetTests
{
    private static List<SpellDto> CreateSpellList(params (Guid Id, SpellLevel Level)[] spells)
    {
        return [..spells.Select(s => new SpellDto
        {
            Id = s.Id,
            Level = (int)s.Level,
            TechnicalName = $"Spell-{s.Id}"
        })];
    }

    [Fact]
    public void Validate_WithinBudget_ReturnsValid()
    {
        var budget = new SpellBudget
        {
            MaxCantrips = 3,
            MaxPreparedSpells = 5,
            MaxSpellLevel = 2
        };

        var selectedIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var allSpells = CreateSpellList(
            (selectedIds[0], SpellLevel.Cantrip),
            (selectedIds[1], SpellLevel.Level1)
        );

        // Pasamos selectedIds como parámetro
        var (isValid, errorKey) = budget.Validate(selectedIds, allSpells);

        Assert.True(isValid);
        Assert.Empty(errorKey);
    }

    [Fact]
    public void Validate_ExceedsCantrips_ReturnsError()
    {
        var ids = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();
        var budget = new SpellBudget
        {
            MaxCantrips = 3,
            MaxPreparedSpells = 10,
            MaxSpellLevel = 9
        };

        var allSpells = ids.Select(id => new SpellDto { Id = id, Level = (int)SpellLevel.Cantrip }).ToList();

        var (isValid, errorKey) = budget.Validate(ids, allSpells);

        Assert.False(isValid);
        // Ahora validamos contra la llave de traducción
        Assert.Equal("Error_MaxCantripsExceeded", errorKey);
    }

    [Fact]
    public void Validate_ExceedsPreparedSpells_ReturnsError()
    {
        var ids = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToList();
        var budget = new SpellBudget
        {
            MaxCantrips = 10,
            MaxPreparedSpells = 5,
            MaxSpellLevel = 9
        };

        var allSpells = ids.Select(id => new SpellDto { Id = id, Level = (int)SpellLevel.Level1 }).ToList();

        var (isValid, errorKey) = budget.Validate(ids, allSpells);

        Assert.False(isValid);
        Assert.Equal("Error_MaxSpellsExceeded", errorKey);
    }

    [Fact]
    public void Validate_SpellLevelTooHigh_ReturnsError()
    {
        var spellId = Guid.NewGuid();
        var budget = new SpellBudget
        {
            MaxCantrips = 10,
            MaxPreparedSpells = 10,
            MaxSpellLevel = 2
        };

        var selectedIds = new List<Guid> { spellId };
        var allSpells = new List<SpellDto>
        {
            new() { Id = spellId, Level = (int) SpellLevel.Level3 }
        };

        var (isValid, errorKey) = budget.Validate(selectedIds, allSpells);

        Assert.False(isValid);
        Assert.Equal("Error_MaxSpellLevelExceeded", errorKey);
    }

    [Fact]
    public void Validate_MixedSelection_WithinBudget_ReturnsValid()
    {
        var cantripIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var spellIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var budget = new SpellBudget
        {
            MaxCantrips = 3,
            MaxPreparedSpells = 5,
            MaxSpellLevel = 3
        };

        var selectedIds = cantripIds.Concat(spellIds).ToList();

        var allSpells = cantripIds.Select(id => new SpellDto { Id = id, Level = (int)SpellLevel.Cantrip })
            .Concat(spellIds.Select(id => new SpellDto { Id = id, Level = (int)SpellLevel.Level2 }))
            .ToList();

        var (isValid, errorKey) = budget.Validate(selectedIds, allSpells);

        Assert.True(isValid);
        Assert.Empty(errorKey);
    }

    [Fact]
    public void SelectedCantripsCount_CountsOnlyLevelZero()
    {
        var cantripId = Guid.NewGuid();
        var spellId = Guid.NewGuid();
        _ = new SpellBudget();
        var selectedIds = new List<Guid> { cantripId, spellId };

        var allSpells = new List<SpellDto>
        {
            new() { Id = cantripId, Level = (int) SpellLevel.Cantrip },
            new() { Id = spellId, Level = (int)SpellLevel.Level1 }
        };

        int count = SpellBudget.SelectedCantripsCount(selectedIds, allSpells);

        Assert.Equal(1, count);
    }

    [Fact]
    public void SelectedSpellsCount_CountsOnlyNonCantrips()
    {
        var cantripId = Guid.NewGuid();
        var spellId1 = Guid.NewGuid();
        var spellId2 = Guid.NewGuid();
        _ = new SpellBudget();

        var selectedIds = new List<Guid> { cantripId, spellId1, spellId2 };

        var allSpells = new List<SpellDto>
        {
            new() { Id = cantripId, Level = (int) SpellLevel.Cantrip },
            new() { Id = spellId1, Level = (int) SpellLevel.Level1 },
            new() { Id = spellId2, Level = (int) SpellLevel.Level2 }
        };

        int count = SpellBudget.SelectedSpellsCount(selectedIds, allSpells);

        Assert.Equal(2, count);
    }

    [Fact]
    public void InitiallyKnownSpellIds_ArePreserved()
    {
        var knownId = Guid.NewGuid();
        var budget = new SpellBudget
        {
            InitiallyKnownSpellIds = [knownId]
        };

        Assert.Single(budget.InitiallyKnownSpellIds);
        Assert.Contains(knownId, budget.InitiallyKnownSpellIds);
    }
}