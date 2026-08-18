using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class ClassLevelProgressionExtractorTests
{
    private readonly ClassLevelProgressionExtractor _sut = new();

    private static readonly string[] Headers = [ "Class", "Level", "ProficiencyBonus", "Feature", "ClassTraits", "Modifiers", "NameES", "DescriptionES" ];

    [Fact]
    public void Extract_CreatesProgressionWithFeatureAndTraits()
    {
        var wb = CreateWorkbook("ClassLevelProgression", Headers,
            ["Wizard", "1", "2", "Spellcasting", "SpellSlots:[4,3,2,1,0,0,0,0,0]|PreparedSpellsCount:3", "[]", "Lanzamiento", "Desc"]);
        var ctx = CreateContext();
        var wizard = new ClassDefinition { Id = Guid.NewGuid(), TechnicalName = "Wizard" };
        ctx.Package.ClassDefinitions.Add(wizard);

        _sut.Extract(wb, ctx);

        var p = ctx.Package.ClassLevelProgressions.Should().ContainSingle().Subject;
        p.ClassDefId.Should().Be(wizard.Id);
        p.Level.Should().Be(1);
        p.Features.Should().ContainSingle(f => f.TechnicalName == "Spellcasting");
        p.Traits.Should().Contain(t => t.Type == ResourceType.SpellSlots && t.SpellSlots!.SequenceEqual(new[] { 4, 3, 2, 1, 0, 0, 0, 0, 0 }));
        p.Traits.Should().Contain(t => t.Type == ResourceType.PreparedSpellsCount && t.Value == "3");
    }

    [Fact]
    public void Extract_MultipleRowsSameLevel_MergesIntoOneProgression()
    {
        var wb = CreateWorkbook("ClassLevelProgression", Headers,
            [ "Wizard", "1", "2", "Spellcasting", "", "[]", "Lanzamiento", "Desc" ],
            [ "Wizard", "1", "2", "ArcaneRecovery", "PreparedSpellsCount:3", "[]", "Recuperación", "Desc" ]);
        var ctx = CreateContext();
        ctx.Package.ClassDefinitions.Add(new ClassDefinition { Id = Guid.NewGuid(), TechnicalName = "Wizard" });

        _sut.Extract(wb, ctx);

        var p = ctx.Package.ClassLevelProgressions.Should().ContainSingle().Subject;
        p.Features.Should().HaveCount(2);
        p.Traits.Should().ContainSingle(t => t.Type == ResourceType.PreparedSpellsCount && t.Value == "3");
    }

    [Fact]
    public void Extract_UnknownClass_SkipsRow()
    {
        var wb = CreateWorkbook("ClassLevelProgression", Headers,
            [ "Unknown", "1", "2", "Feature", "", "[]", "N", "D" ]);
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.ClassLevelProgressions.Should().BeEmpty();
    }
}