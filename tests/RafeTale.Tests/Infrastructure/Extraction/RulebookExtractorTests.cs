using ClosedXML.Excel;
using FluentAssertions;
using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction;
using System.IO;
using Xunit;

namespace RafeTale.Tests.Infrastructure.Extraction;

public class RulebookExtractorTests
{
    [Fact]
    public void Extract_ValidWorkbook_ShouldPopulatePackageWithLinkedGuids()
    {
        // Arrange
        using var workbook = new XLWorkbook();
        
        // 1. Hoja BookInfo
        var bookSheet = workbook.AddWorksheet("BookInfo");
        bookSheet.Cell("A1").Value = "BookId";
        bookSheet.Cell("B1").Value = "SystemId";
        bookSheet.Cell("C1").Value = "Title";
        bookSheet.Cell("D1").Value = "Type";
        bookSheet.Cell("E1").Value = "Version";
        bookSheet.Cell("F1").Value = "Author";
        bookSheet.Cell("G1").Value = "SupportedLanguages";
        bookSheet.Cell("H1").Value = "DefaultLanguage";

        bookSheet.Cell("A2").Value = "dnd5e-srd";
        bookSheet.Cell("B2").Value = "dnd5e";
        bookSheet.Cell("C2").Value = "D&D 5.1 SRD";
        bookSheet.Cell("D2").Value = "CoreSystem";
        bookSheet.Cell("E2").Value = "1.0.0";
        bookSheet.Cell("F2").Value = "RafeTale";
        bookSheet.Cell("G2").Value = "es,en";
        bookSheet.Cell("H2").Value = "es";

        // 2. Hoja Attributes
        var attrSheet = workbook.AddWorksheet("Attributes");
        attrSheet.Cell("A1").Value = "TechnicalName";
        attrSheet.Cell("B1").Value = "DefaultMin";
        attrSheet.Cell("C1").Value = "DefaultMax";
        attrSheet.Cell("D1").Value = "DisplayOrder";

        attrSheet.Cell("A2").Value = "str";
        attrSheet.Cell("B2").Value = 1;
        attrSheet.Cell("C2").Value = 20;
        attrSheet.Cell("D2").Value = 1;

        // 3. Hoja Skills (vinculada a "str")
        var skillSheet = workbook.AddWorksheet("Skills");
        skillSheet.Cell("A1").Value = "SkillId";
        skillSheet.Cell("B1").Value = "AttributeId";

        skillSheet.Cell("A2").Value = "athletics";
        skillSheet.Cell("B2").Value = "str";

        var context = new ExtractionContext(LocLanguage.es);
        var package = context.Package;
        // Act
        var extractor = new RulebookExtractor();
        extractor.Extract(workbook, context);

        // Assert
        package.Rulebook.Should().NotBeNull();
        package.Rulebook!.BookId.Should().Be("dnd5e-srd");
        package.Rulebook.SupportedLanguages.Should().Contain(["es", "en"]);
        package.Rulebook.DefaultLanguage.Should().Be("es");

        package.Attributes.Should().HaveCount(1);
        var strAttr = package.Attributes[0];
        strAttr.TechnicalName.Should().Be("str");
        strAttr.RulebookId.Should().Be(package.Rulebook.Id);

        package.Skills.Should().HaveCount(1);
        var athleticsSkill = package.Skills[0];
        athleticsSkill.TechnicalName.Should().Be("athletics");
        athleticsSkill.AttributeId.Should().Be(strAttr.Id); // Comprueba el enlace correcto de Guids
        athleticsSkill.RulebookId.Should().Be(package.Rulebook.Id);
    }
}