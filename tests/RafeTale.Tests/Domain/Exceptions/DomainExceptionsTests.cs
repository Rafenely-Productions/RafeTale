using System;
using RafeTale.Domain.Exceptions;

namespace RafeTale.Tests.Domain.Exceptions;

public class DomainExceptionsTests
{
    [Fact]
    public void DomainValidationException_WithMessage_ReturnsCorrectMessage()
    {
        // Arrange
        const string message = "Domain validation failed";

        // Act
        var exception = new DomainValidationException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void DomainValidationException_IsExceptionType()
    {
        // Arrange & Act
        var exception = new DomainValidationException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void DataImportException_WithMessage_ReturnsCorrectMessage()
    {
        // Arrange
        const string message = "Data import error";

        // Act
        var exception = new DataImportException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void DataImportException_IsExceptionType()
    {
        // Arrange & Act
        var exception = new DataImportException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void NotFoundException_WithMessage_ReturnsCorrectMessage()
    {
        // Arrange
        const string message = "Resource not found";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void NotFoundException_WithEntityAndKey_ReturnsFormattedMessage()
    {
        // Arrange
        const string entityName = "Character";
        var key = Guid.NewGuid();
        string expectedMessage = $"{entityName} con identificador '{key}' no fue encontrado.";

        // Act
        var exception = new NotFoundException(entityName, key);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void NotFoundException_IsExceptionType()
    {
        // Arrange & Act
        var exception = new NotFoundException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
