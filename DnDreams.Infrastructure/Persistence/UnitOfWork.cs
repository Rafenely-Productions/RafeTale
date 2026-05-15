using DnDreams.Domain.Interfaces;
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
    public IRaceRepository Races { get; }
    public IClassRepository Classes { get; }
    public ICharacterRepository Characters { get; }
    public IFeatureRepository Features { get; }
    public IXpRulesRepository XpRules { get; }
    public IFeatRepository Feats { get; }
    public ISpellRepository Spells { get; }

    public UnitOfWork(DnDreamsDbContext context)
    {
        _context = context;
        Races = new RaceRepository(_context);
        Classes = new ClassRepository(_context);
        Characters = new CharacterRepository(_context);
        Features = new FeatureRepository(_context);
        XpRules = new XpRulesRepository(_context);
        Feats = new FeatRepository(_context);
        Spells = new SpellRepository(_context);
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
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}