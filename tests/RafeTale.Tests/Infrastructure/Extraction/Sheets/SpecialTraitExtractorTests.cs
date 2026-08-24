using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class SpecialTraitExtractorTests
{
    private readonly SpecialTraitExtractor _sut = new();

    [Fact]
    public void Extract_ParsesModifiersAndLinksTrait()
    {
        var wb = CreateWorkbook("Special Traits",
            [ "Trait", "TechnicalName", "DescriptionEN", "Modifiers", "NameES", "DescriptionES" ],
            [ "Darkvision", "Superior", "Desc", "[{\"Type\":\"AttributeBonus\",\"Target\":\"Wisdom\",\"Value\":1}]", "Sup", "Desc" ]);
        var ctx = CreateContext();
        var trait = new Trait { Id = Guid.NewGuid(), TechnicalName = "Darkvision" };
        ctx.Package.Traits.Add(trait);

        _sut.Extract(wb, ctx);

        var special = ctx.Package.SpecialTraits.Should().ContainSingle().Subject;
        special.TraitId.Should().Be(trait.Id);
        special.Modifiers.Should().ContainSingle(m => m.Type == ModifierType.AttributeBonus && m.Target == "Wisdom" && m.Value == 1);
    }

    [Fact]
    public void Extract_InvalidModifiersJson_ReturnsEmptyList()
    {
        var wb = CreateWorkbook("Special Traits",
            [ "Trait", "TechnicalName", "DescriptionEN", "Modifiers", "NameES", "DescriptionES" ],
            [ "T", "S", "D", "{not valid json", "N", "D" ]);
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.SpecialTraits.Single().Modifiers.Should().BeEmpty();
    }
}