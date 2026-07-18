using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DnDreams.Application.Interfaces.ILevelUpService;

namespace DnDreams.Application.Services.DtosServices
{
    public class LevelUpService(IUnitOfWork uow, IService<CharacterDto, Character> characterDtoService,ISpellServiceSystem spellService) : ILevelUpService
    {
        public async Task<LevelUpDraft> PrepareLevelUpAsync(Guid characterId)
        {
            var character = await uow.Characters.GetByIdAsync(characterId)
                ?? throw new Exception("Héroe no encontrado en el plano material.");

            var classDef = await uow.ClassDefinitions.GetByIdAsync(character.ClassDefId, config => config
                .IncludeCollection(x => x.Progressions, p => p.Features))
                ?? throw new Exception("La definición de clase del personaje está corrupta.");

            int nextLevel = character.Level + 1;

            // Buscamos si la progresión de la base de datos para este nivel específico tiene configurado algo especial
            var nextProgression = classDef.Progressions.FirstOrDefault(p => p.Level == nextLevel);

            // Regla General D&D 2024: Se otorgan dotes/ASI en niveles 4, 8, 12, 16 y 19
            bool givesFeatThisLevel = new[] { 4, 8, 12, 16, 19 }.Contains(nextLevel);

            // Verificar si la progresión le permite aprender Spells (Si tiene slots o marcador en la progresión)
            int spellsToLearn = 0;
            // Aquí podrías expandir con tu lógica de tablas: si es Wizard, Cleric, etc., leyendo campos de tu Excel de progresión

            return new LevelUpDraft
            {
                CharacterId = characterId,
                TargetLevel = nextLevel,
                GivesFeat = givesFeatThisLevel,
                SpellsToLearnCount = spellsToLearn
            };
        }

        public async Task<CharacterDto> CommitLevelUpAsync(LevelUpDraft draft)
        {
            // 1. CARGA INICIAL CON TODOS LOS INCLUDES QUE SE VAN A MODIFICAR
            var character = await uow.Characters.GetByIdAsync(draft.CharacterId, config => config
                .Include(c => c.AcquiredFeatures)
                .Include(c => c.CharacterModifiers)
                .Include(c => c.AcquiredFeats) // 🚨 Agregado
                .Include(c => c.KnownSpells))  // 🚨 Agregado
                ?? throw new Exception("Héroe no encontrado al consolidar nivel.");

            var classDef = await uow.ClassDefinitions.GetByIdAsync(character.ClassDefId, config => config
                .IncludeCollection(x => x.Progressions, p => p.Features))!;

            // 1. Aplicar ganancia de puntos de vida (HP)
            character.Level = draft.TargetLevel;
            character.MaxHp += draft.HpGain;
            character.CurrentHp = character.MaxHp;

            // 2. Inyectar automáticamente los Features nativos
            var currentProgression = classDef.Progressions.FirstOrDefault(p => p.Level == character.Level);
            if (currentProgression?.Features != null)
            {
                foreach (var feature in currentProgression.Features)
                {
                    if (!character.AcquiredFeatures.Any(f => f.Id == feature.Id))
                    {
                        character.AcquiredFeatures.Add(feature);
                    }
                }
            }
            await uow.SaveChangesAsync();

            // 3. Modificadores de Atributo (ASI)
            if (draft.GivesFeat && !draft.SelectedFeatId.HasValue)
            {
                if (draft.SelectedAsiOne.HasValue)
                {
                    var m = new CharacterModifier
                    {
                        Id = Guid.NewGuid(),
                        CharacterId = character.Id,
                        Type = ModifierType.AttributeBonus,
                        Target = draft.SelectedAsiOne.Value.ToString(),
                        Value = draft.SelectedAsiTwo.HasValue ? 1 : 2
                    };
                    character.CharacterModifiers.Add(m);
                    uow.ModifyState (m);
                }

                if (draft.SelectedAsiTwo.HasValue && draft.SelectedAsiTwo != draft.SelectedAsiOne)
                {
                    var m = new CharacterModifier
                    {
                        Id = Guid.NewGuid(),
                        CharacterId = character.Id,
                        Type = ModifierType.AttributeBonus,
                        Target = draft.SelectedAsiTwo.Value.ToString(),
                        Value = 1
                    };
                    character.CharacterModifiers.Add(m);
                    uow.ModifyState(m);

                }
            }
            await uow.SaveChangesAsync();

            // 4. Si eligió una Dote específica
            if (draft.SelectedFeatId.HasValue)
            {
                var feat = await uow.Feats.GetByIdAsync(draft.SelectedFeatId.Value);
                if (feat != null)
                {
                    character.AcquiredFeats.Add(feat);
                }
            }

            // 5. Vincular hechizos nuevos aprendidos
            if (draft.SelectedSpellIds != null && draft.SelectedSpellIds.Any())
            {
                foreach (var spellId in draft.SelectedSpellIds)
                {
                    var spell = await uow.Spells.GetByIdAsync(spellId);
                    if (spell != null && !character.KnownSpells.Any(s => s.Id == spellId))
                    {
                        character.KnownSpells.Add(spell);
                    }
                }
            }
            await uow.SaveChangesAsync();

            await spellService.RecalculateMaxSlotsAsync(character.Id);

            try
            {
                await uow.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error al guardar cambios de nivel: {e.Message}");
            }

            return await characterDtoService.ArmDto(character);
        }
        public async Task<CharacterAuditDto> AuditCharacterAsync(Guid characterId)
        {
            var character = await uow.Characters.GetByIdAsync(characterId, config => config
                .Include(c => c.AcquiredFeats)
                .Include(c => c.KnownSpells)
                .Include(c => c.CharacterModifiers)
                .IncludePaths.Add("ClassDef.Progressions"))
                ?? throw new Exception("Personaje ausente.");
            var progressions =character.ClassDef.Progressions;
            var currentProg = character.ClassDef.Progressions.FirstOrDefault(p => p.Level == character.Level);

            int allowedSpells = 0;

            if (currentProg?.Traits != null)
            {
                // Buscamos el rasgo que maneja los conjuros en tu Sorcerer
                var spellcastingTrait = currentProg.Traits.FirstOrDefault(t => t.SpellSlots != null && t.SpellSlots.Any(s => s > 0));
                if (spellcastingTrait != null && int.TryParse(spellcastingTrait.Value, out int count))
                {
                    // 'Value' en tu Sorcerer contiene "PreparedSpellsCount:X", parseamos el entero correspondiente
                    allowedSpells = count;
                }
            }
            int pendingSpells = Math.Max(0, allowedSpells - character.KnownSpells.Count);

            // 3. AUDITAR DOTES DESDE TU LISTA DE RASGOS (CERO HARDCODEO DE NIVELES)
            // Contamos cuántas dotes exige la progresión histórica basándonos en si el nivel otorgó un Feature tipo Feat/ASI
            int expectedFeats = progressions.Count(p => p.Features != null && p.Features.Any(f => f.TechnicalName.Contains("Ability Score Improvement", StringComparison.OrdinalIgnoreCase) || f.TechnicalName.Contains("Feat", StringComparison.OrdinalIgnoreCase)));

            int takenFeats = character.AcquiredFeats.Count;
            int takenAsis = character.CharacterModifiers.Count(m => m.Type == ModifierType.AttributeBonus);

            int pendingFeats = Math.Max(0, expectedFeats - (takenFeats + takenAsis));

            return new CharacterAuditDto
            {
                PendingFeats = pendingFeats,
                PendingSpells = pendingSpells
            };
        }
    }
}