using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class RaceExtractorTests
{
    private readonly RaceExtractor _sut = new();

    private static (ClosedXML.Excel.XLWorkbook wb, ExtractionContext ctx) SetupWithLanguages()
    {
        var wb = CreateWorkbook("Races",
            new[] { "TechnicalName", "CreatureType", "Size", "Speed", "NameES", "DescriptionES", "DescriptionEN", "Languages" },
            new[] { "Elf", "Humanoid", "Medium", "30", "Elfo", "Desc", "Elf desc", "Common,Elvish" });
        var ctx = CreateContext();
        ctx.Package.Languages.Add(new Language { Id = Guid.NewGuid(), TechnicalName = "Common" });
        ctx.Package.Languages.Add(new Language { Id = Guid.NewGuid(), TechnicalName = "Elvish" });
        return (wb, ctx);
    }

    [Fact]
    public void Extract_MapsCreatureTypeSizeSpeedAndLanguages()
    {
        var (wb, ctx) = SetupWithLanguages();

        _sut.Extract(wb, ctx);

        var elf = ctx.Package.Races.Should().ContainSingle().Subject;
        elf.CreatureType.Should().Be(CreatureType.Humanoid);
        elf.Size.Should().Be(SizeCategory.Medium);
        elf.Speed.Should().Be("30");
        elf.Languages.Should().HaveCount(2);
    }

    [Fact]
    public void Extract_UnknownLanguage_IsSkipped()
    {
        var wb = CreateWorkbook("Races",
            new[] { "TechnicalName", "CreatureType", "Size", "Speed", "NameES", "DescriptionES", "DescriptionEN", "Languages" },
            new[] { "Orc", "Humanoid", "Medium", "30", "Orco", "Desc", "Desc", "Klingon" });
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.Races.Single().Languages.Should().BeEmpty();
    }

    [Fact]
    public void Extract_InvalidEnums_FallBackToDefaults()
    {
        var wb = CreateWorkbook("Races",
            new[] { "TechnicalName", "CreatureType", "Size", "Speed", "NameES", "DescriptionES", "DescriptionEN", "Languages" },
            new[] { "X", "NotACreature", "NotASize", "30", "N", "D", "D", "" });
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        var race = ctx.Package.Races.Single();
        race.CreatureType.Should().Be(default(CreatureType));
        race.Size.Should().Be(default(SizeCategory));
    }
}