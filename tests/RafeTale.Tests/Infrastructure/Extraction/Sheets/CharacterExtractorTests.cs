using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class CharacterExtractorTests
{
    private readonly CharacterExtractor _sut = new();

    [Fact]
    public void Extract_ParsesStatsAndLinksRaceClassBackground()
    {
        var wb = CreateWorkbook("Personajes",
            [ "Name", "Race", "Class", "Level", "XP", "Strength", "Intelligence" ],
            [ "Gandalf", "Human", "Wizard", "5", "6500", "10", "16" ]);
        var ctx = CreateContext();
        var race = new Race { Id = Guid.NewGuid(), TechnicalName = "Human" };
        var classDef = new ClassDefinition { Id = Guid.NewGuid(), TechnicalName = "Wizard" };
        var background = new Background { Id = Guid.NewGuid(), TechnicalName = "Soldier" };
        ctx.Package.Races.Add(race);
        ctx.Package.ClassDefinitions.Add(classDef);
        ctx.Package.Backgrounds.Add(background);

        _sut.Extract(wb, ctx);

        var gandalf = ctx.Package.Characters.Should().ContainSingle().Subject;
        gandalf.Level.Should().Be(5);
        gandalf.Experience.Should().Be(6500);
        gandalf.RaceId.Should().Be(race.Id);
        gandalf.ClassDefId.Should().Be(classDef.Id);
        gandalf.BackgroundId.Should().Be(background.Id);
        gandalf.Stats["Strength"].Should().Be(10);
        gandalf.Stats["Intelligence"].Should().Be(16);
    }

    [Fact]
    public void Extract_MissingOptionalSheet_ReturnsWithoutError()
    {
        var wb = new ClosedXML.Excel.XLWorkbook();
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.Characters.Should().BeEmpty();
    }

    [Fact]
    public void Extract_NoBackgroundsAvailable_DoesNotThrow()
    {
        var wb = CreateWorkbook("Personajes",
            [ "Name", "Race", "Class", "Level", "XP" ],
            [ "Gandalf", "Human", "Wizard", "5", "6500" ]);
        var ctx = CreateContext(); // empty Backgrounds — previously crashed with backgrounds[0]

        Action act = () => _sut.Extract(wb, ctx);

        act.Should().NotThrow();
        ctx.Package.Characters.Single().BackgroundId.Should().Be(Guid.Empty);
    }
}