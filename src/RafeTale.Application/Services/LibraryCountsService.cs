using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Interfaces.DtosInterfaces;
using RafeTale.Domain.Entities;

namespace RafeTale.Application.Services;

public class LibraryCountsService : ILibraryCountsService
{
    private readonly IService<ClassDefinitionDto, ClassDefinition> _classService;
    private readonly IService<RaceDto, Race> _raceService;
    private readonly IService<SpellDto, Spell> _spellService;
    private readonly IService<BackgroundDto, Background> _bgService;
    private readonly IService<FeatDto, Feat> _featService;

    public LibraryCountsService(
        IService<ClassDefinitionDto, ClassDefinition> classService,
        IService<RaceDto, Race> raceService,
        IService<SpellDto, Spell> spellService,
        IService<BackgroundDto, Background> bgService,
        IService<FeatDto, Feat> featService)
    {
        _classService = classService;
        _raceService = raceService;
        _spellService = spellService;
        _bgService = bgService;
        _featService = featService;
    }

    public async Task<LibraryCounts> GetCountsAsync()
    {
        // TODO: Reemplazar por CountAsync cuando exista en IUnitOfWork
        var classesTask = _classService.GetAllAsync(null, null);
        var racesTask = _raceService.GetAllAsync(null, null);
        var spellsTask = _spellService.GetAllAsync(null);
        var bgsTask = _bgService.GetAllAsync(null, null);
        var featsTask = _featService.GetAllAsync(null, null);

        await Task.WhenAll(classesTask, racesTask, spellsTask, bgsTask, featsTask);

        return new LibraryCounts(
            (await classesTask).Count,
            (await racesTask).Count,
            (await spellsTask).Count,
            (await bgsTask).Count,
            (await featsTask).Count
        );
    }
}