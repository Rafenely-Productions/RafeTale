using FluentAssertions;
using RafeTale.Domain.ValueObjects;
using Xunit;

namespace RafeTale.Tests.Domain.ValueObjects;

public class DistanceTokenTests
{
    [Theory]
    [InlineData("{d:30_ft}", 30.0, "ft")]
    [InlineData("{d:9_m}", 9.0, "m")]
    [InlineData("{d:1.5_km}", 1.5, "km")]
    [InlineData("{d:10_mi}", 10.0, "mi")]
    [InlineData("{D:25_FT}", 25.0, "ft")]
    public void TryParse_ValidTokens_ShouldReturnTrueAndPopulateValues(string rawToken, double expectedVal, string expectedUnit)
    {
        // Act
        bool success = DistanceToken.TryParse(rawToken, out var token);

        // Assert
        success.Should().BeTrue();
        token.Value.Should().Be(expectedVal);
        token.Unit.Should().Be(expectedUnit);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("30ft")]
    [InlineData("{dist:30}")]
    [InlineData("{d:30_invalid}")]
    public void TryParse_InvalidTokens_ShouldReturnFalse(string? rawToken)
    {
        // Act
        bool success = DistanceToken.TryParse(rawToken!, out var token);

        // Assert
        success.Should().BeFalse();
        token.Value.Should().Be(0.0);
        token.Unit.Should().BeNull();
    }

    [Fact]
    public void Parse_InvalidToken_ShouldFallbackToDefault30Ft()
    {
        // Act
        var token = DistanceToken.Parse("texto_invalido");

        // Assert
        token.Value.Should().Be(30.0);
        token.Unit.Should().Be("ft");
    }

    [Theory]
    [InlineData(30.0, "ft", "m", 9.0)]
    [InlineData(25.0, "ft", "m", 7.5)]
    [InlineData(5.0, "ft", "m", 1.5)]
    [InlineData(9.0, "m", "ft", 30.0)]
    [InlineData(1.5, "m", "ft", 5.0)]
    [InlineData(1.0, "mi", "km", 1.61)]
    [InlineData(1000.0, "m", "km", 1.0)]
    public void ConvertTo_ValidTargetUnits_ShouldCalculateCorrectEquivalence(
        double initialValue,
        string initialUnit,
        string targetUnit,
        double expectedValue)
    {
        // Arrange
        var token = new DistanceToken(initialValue, initialUnit);

        // Act
        var converted = token.ConvertTo(targetUnit);

        // Assert
        converted.Unit.Should().Be(targetUnit);
        converted.Value.Should().Be(expectedValue);
    }
}