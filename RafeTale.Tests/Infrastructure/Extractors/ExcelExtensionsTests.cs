using ClosedXML.Excel;
using RafeTale.Domain.Exceptions;
using RafeTale.Infrastructure.Extractors.Extensions;

namespace RafeTale.Tests.Infrastructure.Extractors;

public class ExcelExtensionsTests
{
    [Fact]
    public void GetSheet_ExistingSheet_ReturnsWorksheet()
    {
        // Arrange
        using var workbook = new XLWorkbook();
        workbook.AddWorksheet("Spells");

        // Act
        var result = workbook.GetSheet("Spells");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Spells");
    }

    [Fact]
    public void GetSheet_MissingSheetRequired_ThrowsDataImportException()
    {
        // Arrange
        using var workbook = new XLWorkbook();

        // Act
        var act = () => workbook.GetSheet("Missing");

        // Assert
        act.Should().Throw<DataImportException>()
            .WithMessage("*La pestaña 'Missing' no existe en el archivo Excel*");
    }

    [Fact]
    public void GetSheet_MissingSheetNotRequired_ReturnsNull()
    {
        // Arrange
        using var workbook = new XLWorkbook();

        // Act
        var result = workbook.GetSheet("Optional", isRequired: false);

        // Assert
        result.Should().BeNull();
    }
}