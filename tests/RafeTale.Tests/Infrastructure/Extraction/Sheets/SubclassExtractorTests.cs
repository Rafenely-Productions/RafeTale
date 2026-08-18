using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class SubclassExtractorTests
{
    private readonly SubclassExtractor _sut = new();

    [Fact]
    public void Extract_LinksToClassDefinition()
    {
        var wb = CreateWorkbook("SubClasses",
            ["Class", "TechnicalName", "DescriptionEN", "NameES", "DescriptionES"],
            ["Wizard", "Evocation", "Desc", "Evocación", "Desc"]);
        var ctx = CreateContext();
        var wizard = new ClassDefinition { Id = Guid.NewGuid(), TechnicalName = "Wizard" };
        ctx.Package.ClassDefinitions.Add(wizard);

        _sut.Extract(wb, ctx);

        var sub = ctx.Package.Subclasses.Should().ContainSingle().Subject;
        sub.ClassDefinition.Should().Be(wizard);
        wizard.Subclasses.Should().ContainSingle(s => s.TechnicalName == "Evocation");
    }
}