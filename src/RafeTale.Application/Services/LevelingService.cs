using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Models;
using RafeTale.Application.Services.DtosServices;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;

namespace RafeTale.Application.Services;

public class LevelingService(IUnitOfWork unitOfWork) : ILevelingService
{
    public async Task<bool> AddExperienceAsync(Guid characterId, int xpAmount)
    {
        // 1. Traer al personaje con sus detalles y rasgos actuales
        // (Nota: necesitaremos mapear el Include de AcquiredFeatures en el repositorio más adelante)
        var character = await unitOfWork.Characters.GetAllWithDetailsAsync();
        var targetChar = character.FirstOrDefault(c => c.Id == characterId);
        var xpRules = await unitOfWork.XpRules.GetXpThresholdsAsync();

        if (targetChar == null) return false;

        await unitOfWork.BeginTransactionAsync();
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
                var progressions = await unitOfWork.ClassLevelProgressions.GetProgressionsByClassAndLevelAsync(targetChar.ClassDefId, targetChar.Level);

                if (progressions != null && progressions.Features.Count != 0)
                {
                    foreach (var feature in progressions.Features)
                    {
                        // Evitamos duplicar si el personaje ya tenía ese rasgo por alguna razón
                        if (!targetChar.AcquiredFeatures.Any(f => f.Id == feature.Id))
                        {
                            targetChar.AcquiredFeatures.Add(feature);

                            foreach (var modData in feature.Modifiers)
                            {
                                var newModifier = new CharacterModifier
                                {
                                    // Id = Guid.Empty, // Asegúrate de que no tenga valor manual
                                    Source = $"Rasgo de Clase: {feature.TechnicalName}",
                                    Type = modData.Type,
                                    Target = modData.Target,
                                    Value = modData.Value,
                                    CharacterId = targetChar.Id // El vínculo explícito
                                };
                                targetChar.CharacterModifiers.Add(newModifier);
                            }
                        }
                    }
                }

                nextLevel++;
            }

            if (leveledUp)
            {
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                return true; // Avisamos que sí hubo subida de nivel
            }

            // Si solo ganó XP pero no subió de nivel, igual guardamos el progreso de la XP
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            return false;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
    public async Task<bool> CommitLevelUpAsync(Guid characterId, int chosenHp, List<CharacterModifier> chosenModifiers, List<Guid> chosenFeatIds, List<Guid> chosenSpellIds)
    {
        var characters = await unitOfWork.Characters.GetAllWithDetailsAsync();
        var targetChar = characters.FirstOrDefault(c => c.Id == characterId);

        if (targetChar == null) return false;

        await unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Aplicar incremento de Puntos de Golpe Maximos (HP)
            // (Asumiendo que tienes una propiedad MaxHitPoints en Character)
            // targetChar.MaxHitPoints += chosenHp; 

            // 2. Inyectar modificadores de atributos o rasgos elegidos (ej: +2 Fuerza)
            foreach (var mod in chosenModifiers)
            {
                targetChar.CharacterModifiers.Add(mod);
            }

            // 3. Vincular los Dotes elegidos desde el catálogo mestro
            if (chosenFeatIds != null)
            {
                var allFeats = await unitOfWork.Feats.GetAllAsync();
                foreach (var featId in chosenFeatIds)
                {
                    var feat = allFeats.FirstOrDefault(f => f!.Id == featId);
                    if (feat != null && !targetChar.AcquiredFeats.Any(f => f.Id == featId))
                    {
                        targetChar.AcquiredFeats.Add(feat);

                        foreach (var modData in feat.Modifiers)
                        {
                            targetChar.CharacterModifiers.Add(new CharacterModifier
                            {
                                Source = $"Dote: {feat.TechnicalName}",
                                Type = modData.Type,
                                Target = modData.Target,
                                Value = modData.Value
                            });
                        }
                    }
                }
            }

            // 4. Vincular los Hechizos elegidos desde el catálogo maestro
            if (chosenSpellIds != null)
            {
                var allSpells = await unitOfWork.Spells.GetAllAsync();
                foreach (var spellId in chosenSpellIds)
                {
                    var spell = allSpells.FirstOrDefault(s => s!.Id == spellId);
                    if (spell != null && !targetChar.KnownSpells.Any(s => s.Id == spellId))
                    {
                        targetChar.KnownSpells.Add(spell);
                    }
                }
            }

            // Guardamos todo con la Unidad de Trabajo
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

}