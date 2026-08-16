using System;
using System.Collections.Generic;
using RafeTale.Application.DTOs;
using RafeTale.Application.Services;
using RafeTale.Domain.Enums;

namespace RafeTale.Tests.Domain.Entities;

public class SpellBudgetTests
{
    [Fact]
    public void DefaultConstructor_InitializesKnownSpellsList_IsEmptyList()
    {
        // Arrange & Act
        var budget = new SpellBudget();

        // Assert
        budget.InitiallyKnownSpellIds.Should().NotBeNull();
        budget.InitiallyKnownSpellIds.Should().BeEmpty();
    }

    [Fact]
    public void SelectedCantripsCount_OnlyCantrips_ReturnsCorrectCount()
    {
        // Arrange
        var budget = new SpellBudget();
        var spells = new List<SpellDto>
        {
            new() { Id = Guid.NewGuid(), Level = (int)SpellLevel.Cantrip },
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Cantrip },
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Level1 }
        };
        var selectedIds = new List<Guid> { spells[0].Id, spells[1].Id, spells[2].Id };

        // Act
        int count = budget.SelectedCantripsCount(selectedIds, spells);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public void SelectedSpellsCount_OnlyLeveledSpells_ReturnsCorrectCount()
    {
        // Arrange
        var budget = new SpellBudget();
        var spells = new List<SpellDto>
        {
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Cantrip },
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Level1 },
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Level2 }
        };
        var selectedIds = new List<Guid> { spells[0].Id, spells[1].Id, spells[2].Id };

        // Act
        int count = budget.SelectedSpellsCount(selectedIds, spells);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public void Validate_WithinBudget_ReturnsValidWithNoError()
    {
        // Arrange
        var budget = new SpellBudget
        {
            MaxCantrips = 2,
            MaxPreparedSpells = 2,
            MaxSpellLevel = 2
        };
        var spells = new List<SpellDto>
        {
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Cantrip },
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Level1 }
        };
        var selectedIds = new List<Guid> { spells[0].Id, spells[1].Id };

        // Act
        var (isValid, errorKey) = budget.Validate(selectedIds, spells);

        // Assert
        isValid.Should().BeTrue();
        errorKey.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ExceedsCantrips_ReturnsMaxCantripsError()
    {
        // Arrange
        var budget = new SpellBudget
        {
            MaxCantrips = 1,
            MaxPreparedSpells = 5,
            MaxSpellLevel = 9
        };
        var spells = new List<SpellDto>
        {
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Cantrip },
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Cantrip }
        };
        var selectedIds = new List<Guid> { spells[0].Id, spells[1].Id };

        // Act
        var (isValid, errorKey) = budget.Validate(selectedIds, spells);

        // Assert
        isValid.Should().BeFalse();
        errorKey.Should().Be("Error_MaxCantripsExceeded");
    }

    [Fact]
    public void Validate_ExceedsSpells_ReturnsMaxSpellsError()
    {
        // Arrange
        var budget = new SpellBudget
        {
            MaxCantrips = 5,
            MaxPreparedSpells = 1,
            MaxSpellLevel = 9
        };
        var spells = new List<SpellDto>
        {
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Level1 },
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Level2 }
        };
        var selectedIds = new List<Guid> { spells[0].Id, spells[1].Id };

        // Act
        var (isValid, errorKey) = budget.Validate(selectedIds, spells);

        // Assert
        isValid.Should().BeFalse();
        errorKey.Should().Be("Error_MaxSpellsExceeded");
    }

    [Fact]
    public void Validate_ExceedsMaxSpellLevel_ReturnsMaxSpellLevelError()
    {
        // Arrange
        var budget = new SpellBudget
        {
            MaxCantrips = 5,
            MaxPreparedSpells = 5,
            MaxSpellLevel = 1
        };
        var spells = new List<SpellDto>
        {
            new() { Id = Guid.NewGuid(), Level = (int) SpellLevel.Level2 }
        };
        var selectedIds = new List<Guid> { spells[0].Id };

        // Act
        var (isValid, errorKey) = budget.Validate(selectedIds, spells);

        // Assert
        isValid.Should().BeFalse();
        errorKey.Should().Be("Error_MaxSpellLevelExceeded");
    }
}
