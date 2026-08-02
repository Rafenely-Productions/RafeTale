using Rafedream.Application.DTOs;

namespace Rafedream.Application.Interfaces;

public interface ILibraryCountsService
{
    /// <summary>
    /// Carga los contadores de cada categoría de la biblioteca.
    /// TODO: Optimizar con CountAsync en repositorios para no cargar todos los DTOs en memoria.
    /// </summary>
    Task<LibraryCounts> GetCountsAsync();
}