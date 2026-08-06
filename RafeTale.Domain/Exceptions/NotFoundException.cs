namespace RafeTale.Domain.Exceptions;

/// <summary>
/// Lanzada cuando una entidad o recurso requerido no se encuentra en la base de datos.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object key)
        : base($"{entityName} con identificador '{key}' no fue encontrado.") { }
}
