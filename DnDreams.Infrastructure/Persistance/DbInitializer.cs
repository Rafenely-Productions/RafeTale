using DnDreams.Domain;
using DnDreams.Domain.Entities;

namespace DnDreams.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Seed(DnDreamsDbContext context)
    {
        // Solo insertamos si la base de datos está vacía
        if (context.ClassDefinitions.Any()) return;

        var fighter = new ClassDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Guerrero",
            HitDie = "d10",
            Progression = new List<ClassLevelProgression>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Level = 1,
                    Features = new List<Feature> {
                        new() { Id = Guid.NewGuid(), Name = "Estilo de Combate", Description = "Elige una especialidad", RequiresChoice = true },
                        new() { Id = Guid.NewGuid(), Name = "Recuperación (Second Wind)", Description = "Recuperas vida con acción adicional", RequiresChoice = false }
                    }
                },
                new() {
                    Id = Guid.NewGuid(),
                    Level = 2,
                    Features = new List<Feature> {
                        new() { Id = Guid.NewGuid(), Name = "Acción Súbita (Action Surge)", Description = "Una acción adicional en tu turno", RequiresChoice = false }
                    }
                }
            }
        };

        context.ClassDefinitions.Add(fighter);
        context.SaveChanges();
    }
}