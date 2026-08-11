using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class TraitExtractorTests
{
    private readonly TraitExtractor _sut = new();

    [Fact]
    public void Extract_LinksToRaceOrSubrace()
    {
        var wb = CreateWorkbook("Traits",
            new[] { "TechnicalName", "RequiredLevel", "Race", "Subrace", "NameES", "DescriptionES", "DescriptionEN" },
            new[] { "HumanTrait", "1", "Human", "", "Rasgo", "Desc", "Desc" },
            new[] { "SubTrait", "1", "", "HighElf", "Rasgo", "Desc", "Desc" });
        var ctx = CreateContext();
        ctx.Package.Races.Add(new Race { Id = Guid.NewGuid(), TechnicalName = "Human" });
        ctx.Package.SubRaces.Add(new SubRace { Id = Guid.NewGuid(), TechnicalName = "HighElf" });

        _sut.Extract(wb, ctx);

        ctx.Package.Traits.Should().HaveCount(2);
        ctx.Package.Traits.Should().Contain(t => t.TechnicalName == "HumanTrait" && t.Race != null);
        ctx.Package.Traits.Should().Contain(t => t.TechnicalName == "SubTrait" && t.Subrace != null);
    }

    [Fact]
    public void Extract_ParsesRequiredLevel()
    {
        var wb = CreateWorkbook("Traits",
            new[] { "TechnicalName", "RequiredLevel", "Race", "Subrace", "NameES", "DescriptionES", "DescriptionEN" },
            new[] { "T", "5", "", "", "N", "D", "D" });
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.Traits.Single().RequiredLevel.Should().Be(5);
    }
}