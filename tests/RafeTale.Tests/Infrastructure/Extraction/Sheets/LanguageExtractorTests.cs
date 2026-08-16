using FluentAssertions;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class LanguageExtractorTests
{
    private readonly LanguageExtractor _sut = new();

    [Fact]
    public void Extract_CreatesLanguagesAndLocalization()
    {
        var wb = CreateWorkbook("Languages",
            new[] { "TechnicalName", "DescriptionEN", "NameES", "DescriptionES" },
            new[] { "Common", "Common language", "Común", "Idioma común" });
        var context = CreateContext();

        _sut.Extract(wb, context);

        context.Package.Languages.Should().ContainSingle(l => l.TechnicalName == "Common");
        context.Localization.GetAll().Should().NotBeEmpty();
    }

    [Fact]
    public void Extract_MissingRequiredSheet_Throws()
    {
        var wb = new ClosedXML.Excel.XLWorkbook();
        var context = CreateContext();

        Action act = () => _sut.Extract(wb, context);

        act.Should().Throw<Exception>().WithMessage("*Languages*");
    }
}