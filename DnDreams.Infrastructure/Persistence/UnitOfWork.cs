using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DnDreamsDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    // Inicializamos las propiedades de los repositorios de golpe pasándoles el mismo contexto
    public IRepository<Race> Races { get; }
    public IClassDefinitionRepository ClassDefinitions { get; }
    public IClassLevelProgressionRepository ClassLevelProgressions { get; }
    public ICharacterRepository Characters { get; }
    public IFeatureRepository Features { get; }
    public IXpRulesRepository XpRules { get; }
    public IFeatRepository Feats { get; }
    public ISpellRepository Spells { get; }
    public IItemTemplateRepository ItemTemplates { get; }

    public ILocalizedContentRepository LocalizedContents { get; }
    public IRepository<Language> Languages { get; }
    public IRepository<Trait> Traits { get; }
    public IRepository<SubRace> SubRaces { get; }
    public IRepository<SchoolOfMagic> SchoolsOfMagic { get; }
    public IRepository<Background> Backgrounds {get;}

    public UnitOfWork(DnDreamsDbContext context)
    {
        _context = context;
        Races = new Repository<Race>(_context);
        ClassDefinitions = new ClassDefinitionRepository(_context);
        ClassLevelProgressions = new ClassLevelProgressionRepository(_context);
        Characters = new CharacterRepository(_context);
        Features = new FeatureRepository(_context);
        XpRules = new XpRulesRepository(_context);
        Feats = new FeatRepository(_context);
        Backgrounds = new Repository<Background>(_context);
        Spells = new SpellRepository(_context);
        ItemTemplates = new ItemTemplateRepository(_context);
        LocalizedContents = new LocalizedContentRepository(_context);
        Languages = new Repository<Language>(_context);
        Traits = new Repository<Trait>(_context);
        SubRaces = new Repository<SubRace>(_context);
        SchoolsOfMagic = new Repository<SchoolOfMagic>(_context);
    }

    public async Task BeginTransactionAsync()
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}