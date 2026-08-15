using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class SubclassLevelProgressionExtractorTests
{
    private readonly SubclassLevelProgressionExtractor _sut = new();

    [Fact]
    public void Extract_CreatesProgressionAndUpdatesSubclass()
    {
        var wb = CreateWorkbook("SubClassLevelProgresion",
            new[] { "Class", "Subclass", "Feature", "Level", "Modifiers", "NameES", "DescriptionES" },
            new[] { "Wizard", "Evocation", "SculptSpells", "3", "[]", "Esculpir", "Desc" });
        var ctx = CreateContext();
        var evocation = new Subclass { Id = Guid.NewGuid(), TechnicalName = "Evocation" };
        ctx.Package.Subclasses.Add(evocation);

        _sut.Extract(wb, ctx);

        var p = ctx.Package.SubclassLevelProgressions.Should().ContainSingle().Subject;
        p.SubclassId.Should().Be(evocation.Id);
        p.Features.Should().ContainSingle(f => f.TechnicalName == "SculptSpells");
        evocation.Progressions.Should().ContainSingle(x => x.Id == p.Id);
    }

    [Fact]
    public void Extract_UnknownSubclass_SkipsRow()
    {
        var wb = CreateWorkbook("SubClassLevelProgresion",
            new[] { "Class", "Subclass", "Feature", "Level", "Modifiers", "NameES", "DescriptionES" },
            new[] { "Wizard", "Necromancy", "GrimHarvest", "2", "[]", "Cosecha", "Desc" });
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.SubclassLevelProgressions.Should().BeEmpty();
    }
}

public class XpRuleExtractorTests
{
    private readonly XpRuleExtractor _sut = new();

    [Fact]
    public void Extract_ParsesLevelXpAndBonus()
    {
        var wb = CreateWorkbook("ReglasXP",
            new[] { "Level", "RequiredXp", "Bonus" },
            new[] { "1", "0", "0" },
            new[] { "2", "300", "5" });
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.XpRules.Should().HaveCount(2);
        ctx.Package.XpRules.Should().Contain(r => r.Level == 2 && r.RequiredXp == 300 && r.Bonus == 5);
    }
}