using DnDreams.Domain.Entities;
using DnDreams.Application.Models;

namespace DnDreams.Application.Services;

public class LevelingService
{
    public List<LevelUpRequirement> GetPendingRequirements(Character character, ClassDefinition classDef, int targetLevel)
    {
        var requirements = new List<LevelUpRequirement>();

        // 1. Buscar qué Features otorga la clase en ese nivel
        var progression = classDef.Progression.FirstOrDefault(p => p.Level == targetLevel);

        if (progression == null) return requirements;

        foreach (var feature in progression.Features)
        {
            if (feature.RequiresChoice)
            {
                requirements.Add(new LevelUpRequirement
                {
                    FeatureName = feature.Name,
                    Description = feature.Description,
                    Type = RequirementType.Choice,
                    IsCompleted = false // Esto obligará a la UI a mostrar el selector
                });
            }
            else
            {
                requirements.Add(new LevelUpRequirement
                {
                    FeatureName = feature.Name,
                    Description = feature.Description,
                    Type = RequirementType.Informational,
                    IsCompleted = true
                });
            }
        }

        // 2. Regla especial: Mejora de Atributos (ASI) cada 4 niveles (regla general de D&D)
        if (targetLevel % 4 == 0)
        {
            requirements.Add(new LevelUpRequirement
            {
                FeatureName = "Mejora de Puntuación de Característica",
                Description = "Aumenta un atributo en +2 o dos en +1",
                Type = RequirementType.StatIncrease,
                IsCompleted = false
            });
        }

        return requirements;
    }
}