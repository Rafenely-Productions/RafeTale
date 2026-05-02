using Microsoft.EntityFrameworkCore;
using DnDreams.Domain;
using DnDreams.Domain.Entities;

namespace DnDreams.Infrastructure.Persistence;

public class DnDreamsDbContext : DbContext
{
    public DbSet<Character> Characters { get; set; }
    public DbSet<ClassDefinition> ClassDefinitions { get; set; }
    public DbSet<Feature> Features { get; set; }

    public DnDreamsDbContext(DbContextOptions<DnDreamsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aquí configuramos reglas específicas, por ejemplo:
        // Un personaje puede tener muchos niveles de clase.
        modelBuilder.Entity<Character>()
            .HasMany(c => c.ClassLevels)
            .WithOne();

        // Las clases tienen una progresión de niveles.
        modelBuilder.Entity<ClassDefinition>()
            .HasMany(cd => cd.Progression)
            .WithOne();
    }
}