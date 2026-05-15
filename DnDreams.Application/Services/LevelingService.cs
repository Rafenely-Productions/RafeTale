using DnDreams.Application.Interfaces;
using DnDreams.Application.Models;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;

namespace DnDreams.Application.Services;

public class LevelingService : ILevelingService
{
    private readonly IUnitOfWork _unitOfWork;

    public LevelingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AddExperienceAsync(Guid characterId, int xpAmount)
    {
        // 1. Traer al personaje con sus detalles y rasgos actuales
        // (Nota: necesitaremos mapear el Include de AcquiredFeatures en el repositorio más adelante)
        var character = await _unitOfWork.Characters.GetAllWithDetailsAsync();
        var targetChar = character.FirstOrDefault(c => c.Id == characterId);
        var xpRules = await _unitOfWork.XpRules.GetXpThresholdsAsync();

        if (targetChar == null) return false;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            targetChar.Experience += xpAmount;
            bool leveledUp = false;
            int nextLevel = targetChar.Level + 1;

            // 2. Bucle por si gana tanta XP que sube más de un nivel de golpe
            while (xpRules.ContainsKey(nextLevel) && targetChar.Experience >= xpRules[nextLevel])
            {
                targetChar.Level = nextLevel;
                leveledUp = true;

                // 3. Buscar qué rasgos otorga su clase en este nuevo nivel
                // Para esto ocupamos consultar la tabla de progresiones que cargamos desde el Excel
                var progressions = await _unitOfWork.Classes.GetProgressionsByClassAndLevelAsync(targetChar.ClassDefId, targetChar.Level);

                if (progressions != null && progressions.Features.Any())
                {
                    foreach (var feature in progressions.Features)
                    {
                        // Evitamos duplicar si el personaje ya tenía ese rasgo por alguna razón
                        if (!targetChar.AcquiredFeatures.Any(f => f.Id == feature.Id))
                        {
                            targetChar.AcquiredFeatures.Add(feature);

                            foreach (var modData in feature.Modifiers)
                            {
                                if (Enum.TryParse<ModifierType>(modData.Type, out var parsedType))
                                {
                                    var newModifier = new CharacterModifier
                                    {
                                        // Id = Guid.Empty, // Asegúrate de que no tenga valor manual
                                        Source = $"Rasgo de Clase: {feature.Name}",
                                        Type = parsedType,
                                        Target = modData.Target,
                                        Value = modData.Value,
                                        CharacterId = targetChar.Id // El vínculo explícito
                                    };
                                    targetChar.ActiveModifiers.Add(newModifier);
                                }
                            }
                        }
                    }
                }

                nextLevel++;
            }

            if (leveledUp)
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return true; // Avisamos que sí hubo subida de nivel
            }

            // Si solo ganó XP pero no subió de nivel, igual guardamos el progreso de la XP
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return false;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
    public List<LevelUpRequirement> GetPendingRequirements(Character character, ClassDefinition classDef, int targetLevel)
    {
        var requirements = new List<LevelUpRequirement>();

        // 1. Buscar qué Features otorga la clase en ese nivel
        var progression = classDef.Progressions.FirstOrDefault(p => p.Level == targetLevel);

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
    public async Task<bool> CommitLevelUpAsync(Guid characterId, int chosenHp, List<CharacterModifier> chosenModifiers, List<Guid> chosenFeatIds, List<Guid> chosenSpellIds)
    {
        var characters = await _unitOfWork.Characters.GetAllWithDetailsAsync();
        var targetChar = characters.FirstOrDefault(c => c.Id == characterId);

        if (targetChar == null) return false;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Aplicar incremento de Puntos de Golpe Maximos (HP)
            // (Asumiendo que tienes una propiedad MaxHitPoints en Character)
            // targetChar.MaxHitPoints += chosenHp; 

            // 2. Inyectar modificadores de atributos o rasgos elegidos (ej: +2 Fuerza)
            foreach (var mod in chosenModifiers)
            {
                targetChar.ActiveModifiers.Add(mod);
            }

            // 3. Vincular los Dotes elegidos desde el catálogo mestro
            if (chosenFeatIds != null)
            {
                var allFeats = await _unitOfWork.Feats.GetAllAsync();
                foreach (var featId in chosenFeatIds)
                {
                    var feat = allFeats.FirstOrDefault(f => f.Id == featId);
                    if (feat != null && !targetChar.AcquiredFeats.Any(f => f.Id == featId))
                    {
                        targetChar.AcquiredFeats.Add(feat);

                        foreach (var modData in feat.Modifiers)
                        {
                            // Mapeamos el string del tipo al Enum real de la base de datos
                            if (Enum.TryParse<ModifierType>(modData.Type, out var parsedType))
                            {
                                targetChar.ActiveModifiers.Add(new CharacterModifier
                                {
                                    Source = $"Dote: {feat.Name}",
                                    Type = parsedType,
                                    Target = modData.Target,
                                    Value = modData.Value
                                });
                            }
                        }
                    }
                }
            }

            // 4. Vincular los Hechizos elegidos desde el catálogo maestro
            if (chosenSpellIds != null)
            {
                var allSpells = await _unitOfWork.Spells.GetAllAsync();
                foreach (var spellId in chosenSpellIds)
                {
                    var spell = allSpells.FirstOrDefault(s => s.Id == spellId);
                    if (spell != null && !targetChar.KnownSpells.Any(s => s.Id == spellId))
                    {
                        targetChar.KnownSpells.Add(spell);
                    }
                }
            }

            // Guardamos todo con la Unidad de Trabajo
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

}