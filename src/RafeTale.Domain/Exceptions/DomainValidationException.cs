namespace RafeTale.Domain.Exceptions;

/// <summary>
/// Lanzada cuando el estado del dominio no permite realizar una operación.
/// </summary>
public class DomainValidationException(string message) : Exception(message)
{
}
