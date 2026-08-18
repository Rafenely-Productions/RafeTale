using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class SubRaceExtractorTests
{
    private readonly SubRaceExtractor _sut = new();

    [Fact]
    public void Extract_LinksToParentRace()
    {
        var wb = CreateWorkbook("Sub Races",
            ["Race", "TechnicalName", "NameES", "Unused", "DescriptionES", "DescriptionEN" ],
            ["Elf", "HighElf", "Alto elfo", "", "Desc", "High elf" ]);
        var ctx = CreateContext();
        var elf = new Race { Id = Guid.NewGuid(), TechnicalName = "Elf" };
        ctx.Package.Races.Add(elf);

        _sut.Extract(wb, ctx);

        var sub = ctx.Package.SubRaces.Should().ContainSingle().Subject;
        sub.TechnicalName.Should().Be("HighElf");
        sub.RaceId.Should().Be(elf.Id);
    }

    [Fact]
    public void Extract_UnknownParentRace_LeavesRaceIdEmpty()
    {
        var wb = CreateWorkbook("Sub Races",
            ["Race", "TechnicalName", "NameES", "Unused", "DescriptionES", "DescriptionEN" ],
            ["Dragonborn", "Draco", "Draco", "", "Desc", "Desc" ]);
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.SubRaces.Single().RaceId.Should().Be(Guid.Empty);
    }
}