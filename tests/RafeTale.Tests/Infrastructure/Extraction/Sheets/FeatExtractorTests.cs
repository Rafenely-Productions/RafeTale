using FluentAssertions;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class FeatExtractorTests
{
    private readonly FeatExtractor _sut = new();

    [Fact]
    public void Extract_ParsesPrerequisiteModifiersAndCategory()
    {
        var wb = CreateWorkbook("Feats",
            ["TechnicalName", "Prerequisite", "Modifiers", "Category", "DescriptionEN", "NameES", "DescriptionES"],
            [ "Tough", "[{\"Type\":\"AttributeMinimum\",\"Target\":\"Constitution\",\"Value\":13}]",
                    "[{\"Type\":\"AttributeBonus\",\"Target\":\"Constitution\",\"Value\":2}]", "General", "Desc", "Robusto", "Desc" ]);
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        var feat = ctx.Package.Feats.Should().ContainSingle().Subject;
        feat.Category.Should().Be(CategoryFeat.General);
        feat.Prerequisite.Should().ContainSingle(p => p.Type == FeatPrerequisiteType.AttributeMinimum && p.Value == 13);
        feat.Modifiers.Should().ContainSingle(m => m.Type == ModifierType.AttributeBonus && m.Value == 2);
    }

    [Fact]
    public void Extract_NoneValues_ParseAsNull()
    {
        var wb = CreateWorkbook("Feats",
            ["TechnicalName", "Prerequisite", "Modifiers", "Category", "DescriptionEN", "NameES", "DescriptionES"],
            ["Lucky", "none", "none", "General", "Desc", "Suerte", "Desc"]);
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        var feat = ctx.Package.Feats.Single();
        feat.Prerequisite.Should().BeEmpty();
        feat.Modifiers.Should().BeEmpty();
    }
}