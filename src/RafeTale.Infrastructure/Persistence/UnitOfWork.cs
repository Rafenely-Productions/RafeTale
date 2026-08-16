using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly RafeTaleDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    // Inicializamos las propiedades de los repositorios de golpe pasándoles el mismo contexto
    public IRaceRepository Races { get; }
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
    public IRepository<Background> Backgrounds { get; }
    public IRepository<Subclass> Subclasses { get; }
    public IRepository<SubclassLevelProgression> SubclassLevelProgressions { get; }
    public IRepository<Skill> Skills { get; }

    public UnitOfWork(RafeTaleDbContext context)
    {
        _context = context;
        Races = new RaceRepository(_context);
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
        Subclasses = new Repository<Subclass>(_context);
        SubclassLevelProgressions = new Repository<SubclassLevelProgression>(_context);
        Skills = new Repository<Skill>(_context);
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
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {
            Console.WriteLine($"🚨 Concurrencia detectada, aplicando bypass automático: {ex.Message}");

            // Forzamos al ChangeTracker a resolver el conflicto usando los valores del cliente (tu UI)
            // para que sobrescriba lo que sea que esté bloqueando a SQLite
            foreach (var entry in ex.Entries)
            {
                var proposedValues = entry.CurrentValues;
                var databaseValues = await entry.GetDatabaseValuesAsync();

                if (databaseValues == null)
                {
                    // Si el registro ya no existe en la base de datos física, lo desvinculamos del tracker
                    entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
                else
                {
                    // Actualizamos los valores originales del tracker para que coincidan con la BD
                    // y que EF Core crea que ya leyó la última versión disponible
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }

            // Reintentamos el guardado de forma segura con el tracker limpio
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public void ModifyState(object m)
    {
        _context.Entry(m).State = Microsoft.EntityFrameworkCore.EntityState.Added;
    }

    public void DetachOrUnchangeEntities<T>() where T : class
    {
        // Buscamos en el ChangeTracker todas las entidades cargadas del tipo T
        // que EF Core crea erróneamente que fueron Modificadas o Borradas
        var entries = _context.ChangeTracker.Entries<T>()
            .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Modified ||
                        e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted);

        foreach (var entry in entries)
        {
            // Forzamos su estado a 'Unchanged' (Sin Cambios) para que SQLite no intente 
            // lanzar un UPDATE/DELETE fantasma sobre el catálogo global (Hechizo/Dote)
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        }
    }
    public void SetUnchangedState(object entity)
    {
        _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
    }

    public void PureCleanTrackerForCharacter()
    {
        // Obtenemos todas las entradas del Change Tracker que EF Core piensa que cambiaron
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Modified ||
                        e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted);

        foreach (var entry in entries)
        {
            // 🚨 REGLA DE ORO: Si la entidad que se quiere modificar/borrar NO es el Character, 
            // ni un modificador nuevo, ni un inventario... es decir, es un objeto de catálogo 
            // (Clase, Hechizo, Rasgo, Dote), forzamos su estado a 'Unchanged'.
            if (entry.Entity is not Character &&
                entry.Entity is not CharacterModifier &&
                entry.Entity is not CharacterSpellSlots &&
                entry.Entity is not CharacterInventory)
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            }
        }
    }
    public async Task<bool> SpellSlotExistsAsync(Guid id)
    {
        return await _context.CharacterSpellSlots.AnyAsync(x => x.Id == id);
    }
    public void TrackNewSpellSlot(CharacterSpellSlots cps)
    {
        _context.CharacterSpellSlots.Add(cps);
    }
}