namespace DnDreams.Domain.Exceptions;

/// <summary>
/// Lanzada cuando el estado del dominio no permite realizar una operación.
/// </summary>
public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message) { }
}
