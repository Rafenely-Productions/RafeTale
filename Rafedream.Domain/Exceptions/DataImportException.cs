namespace Rafedream.Domain.Exceptions;

/// <summary>
/// Lanzada cuando ocurre un error durante la importación de datos externos (Excel, etc.).
/// </summary>
public class DataImportException : Exception
{
    public DataImportException(string message) : base(message) { }
}
