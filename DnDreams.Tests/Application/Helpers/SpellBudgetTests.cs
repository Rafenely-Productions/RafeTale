using DnDreams.Application.DTOs;
using DnDreams.Application.Services;
using DnDreams.Domain.Enums;

namespace DnDreams.Tests.Application.Helpers;

public class SpellBudgetTests
{
    private static List<SpellDto> CreateSpellList(params (Guid Id, SpellLevel Level)[] spells)
    {
        return spells.Select(s => new SpellDto
        {
            Id = s.Id,
            Level = s.Level,
            Name = $"Spell-{s.Id}"
        }).ToList();
    }

    [Fact]
    public void Validate_WithinBudget_ReturnsValid()
    {
        var budget = new SpellBudget
        {
            MaxCantrips = 3,
            MaxPreparedSpells = 5,
            MaxSpellLevel = 2,
            CurrentSelectionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        var allSpells = CreateSpellList(
            (budget.CurrentSelectionIds[0], SpellLevel.Cantrip),
            (budget.CurrentSelectionIds[1], SpellLevel.Level1)
        );

        var (isValid, error) = budget.Validate(allSpells);

        Assert.True(isValid);
        Assert.Empty(error);
    }

    [Fact]
    public void Validate_ExceedsCantrips_ReturnsError()
    {
        var ids = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();
        var budget = new SpellBudget
        {
            MaxCantrips = 3,
            MaxPreparedSpells = 10,
            MaxSpellLevel = 9,
            CurrentSelectionIds = ids
        };

        var allSpells = ids.Select(id => new SpellDto { Id = id, Level = SpellLevel.Cantrip }).ToList();

        var (isValid, error) = budget.Validate(allSpells);

        Assert.False(isValid);
        Assert.Contains("trucos", error);
    }

    [Fact]
    public void Validate_ExceedsPreparedSpells_ReturnsError()
    {
        var ids = Enumerable.Range(0, 6).Select(_ => Guid.NewGuid()).ToList();
        var budget = new SpellBudget
        {
            MaxCantrips = 10,
            MaxPreparedSpells = 5,
            MaxSpellLevel = 9,
            CurrentSelectionIds = ids
        };

        var allSpells = ids.Select(id => new SpellDto { Id = id, Level = SpellLevel.Level1 }).ToList();

        var (isValid, error) = budget.Validate(allSpells);

        Assert.False(isValid);
        Assert.Contains("conjuros preparados", error);
    }

    [Fact]
    public void Validate_SpellLevelTooHigh_ReturnsError()
    {
        var spellId = Guid.NewGuid();
        var budget = new SpellBudget
        {
            MaxCantrips = 10,
            MaxPreparedSpells = 10,
            MaxSpellLevel = 2,
            CurrentSelectionIds = new List<Guid> { spellId }
        };

        var allSpells = new List<SpellDto>
        {
            new() { Id = spellId, Level = SpellLevel.Level3 }
        };

        var (isValid, error) = budget.Validate(allSpells);

        Assert.False(isValid);
        Assert.Contains("nivel superior", error);
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
            MaxSpellLevel = 3,
            CurrentSelectionIds = cantripIds.Concat(spellIds).ToList()
        };

        var allSpells = cantripIds.Select(id => new SpellDto { Id = id, Level = SpellLevel.Cantrip })
            .Concat(spellIds.Select(id => new SpellDto { Id = id, Level = SpellLevel.Level2 }))
            .ToList();

        var (isValid, error) = budget.Validate(allSpells);

        Assert.True(isValid);
        Assert.Empty(error);
    }

    [Fact]
    public void SelectedCantripsCount_CountsOnlyLevelZero()
    {
        var cantripId = Guid.NewGuid();
        var spellId = Guid.NewGuid();
        var budget = new SpellBudget
        {
            CurrentSelectionIds = new List<Guid> { cantripId, spellId }
        };

        var allSpells = new List<SpellDto>
        {
            new() { Id = cantripId, Level = SpellLevel.Cantrip },
            new() { Id = spellId, Level = SpellLevel.Level1 }
        };

        int count = budget.SelectedCantripsCount(allSpells);

        Assert.Equal(1, count);
    }

    [Fact]
    public void SelectedSpellsCount_CountsOnlyNonCantrips()
    {
        var cantripId = Guid.NewGuid();
        var spellId1 = Guid.NewGuid();
        var spellId2 = Guid.NewGuid();
        var budget = new SpellBudget
        {
            CurrentSelectionIds = new List<Guid> { cantripId, spellId1, spellId2 }
        };

        var allSpells = new List<SpellDto>
        {
            new() { Id = cantripId, Level = SpellLevel.Cantrip },
            new() { Id = spellId1, Level = SpellLevel.Level1 },
            new() { Id = spellId2, Level = SpellLevel.Level2 }
        };

        int count = budget.SelectedSpellsCount(allSpells);

        Assert.Equal(2, count);
    }

    [Fact]
    public void InitiallyKnownSpellIds_ArePreserved()
    {
        var knownId = Guid.NewGuid();
        var budget = new SpellBudget
        {
            InitiallyKnownSpellIds = new List<Guid> { knownId },
            CurrentSelectionIds = new List<Guid> { knownId }
        };

        Assert.Single(budget.InitiallyKnownSpellIds);
        Assert.Contains(knownId, budget.InitiallyKnownSpellIds);
    }
}