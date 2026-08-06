using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Interfaces.DtosInterfaces;
using RafeTale.Domain.Entities;

namespace RafeTale.Application.Services;

public class LibraryDataService : ILibraryDataService
{
    private readonly IService<ClassDefinitionDto, ClassDefinition> _classService;
    private readonly IService<RaceDto, Race> _raceService;
    private readonly IService<SpellDto, Spell> _spellService;
    private readonly IService<FeatDto, Feat> _featService;
    private readonly IService<BackgroundDto, Background> _backgroundService;

    public LibraryDataService(
        IService<ClassDefinitionDto, ClassDefinition> classService,
        IService<RaceDto, Race> raceService,
        IService<SpellDto, Spell> spellService,
        IService<FeatDto, Feat> featService,
        IService<BackgroundDto, Background> backgroundService)
    {
        _classService = classService;
        _raceService = raceService;
        _spellService = spellService;
        _featService = featService;
        _backgroundService = backgroundService;
    }

    public async Task<IReadOnlyList<ClassDefinitionDto>> GetClassesAsync()
        => await _classService.GetAllAsync(null, null);

    public async Task<IReadOnlyList<RaceDto>> GetRacesAsync()
        => await _raceService.GetAllAsync(null, includes: query => query
            .Include(x => x.Languages)
            .IncludeCollection(x => x.SubRaces, z => z.Traits)
            .Include(x => x.Traits));

    public async Task<IReadOnlyList<SpellDto>> GetSpellsAsync()
        => await _spellService.GetAllAsync(null);

    public async Task<IReadOnlyList<FeatDto>> GetFeatsAsync()
        => await _featService.GetAllAsync(null, null);

    public async Task<IReadOnlyList<BackgroundDto>> GetBackgroundsAsync()
        => await _backgroundService.GetAllAsync(null, null);
}