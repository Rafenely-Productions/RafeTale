using RafeTale.Application.DTOs;
using RafeTale.Domain.Entities;

namespace RafeTale.Application.Interfaces;

public interface ILibraryDataService
{
    Task<IReadOnlyList<ClassDefinitionDto>> GetClassesAsync();
    Task<IReadOnlyList<RaceDto>> GetRacesAsync();
    Task<IReadOnlyList<SpellDto>> GetSpellsAsync();
    Task<IReadOnlyList<FeatDto>> GetFeatsAsync();
    Task<IReadOnlyList<BackgroundDto>> GetBackgroundsAsync();
}