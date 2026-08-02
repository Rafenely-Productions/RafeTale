using Rafedream.Application.DTOs;
using Rafedream.Application.Interfaces;
using Rafedream.Application.Interfaces.DtosInterfaces;
using Rafedream.Domain.Entities;
using Rafedream.Domain.Enums;
using Rafedream.Domain.Interfaces;
using Rafedream.Domain.Exceptions;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rafedream.Application.Services.DtosServices
{
    public class LevelUpService(IUnitOfWork uow, IService<CharacterDto, Character> characterDtoService, ISpellServiceSystem spellService, ILogger<LevelUpService> logger) : ILevelUpService
    {
        public async Task<LevelUpDraft> PrepareLevelUpAsync(Guid characterId)
        {
            logger.LogInformation("Preparando borrador de subida de nivel para el personaje con ID: {CharacterId}", characterId);

            var character = await uow.Characters.GetByIdAsync(characterId, config => config.Include(c => c.KnownSpells))
                ?? throw new NotFoundException("Personaje", characterId);

            var classDef = await uow.ClassDefinitions.GetByIdAsync(character.ClassDefId, config => config
                .IncludeCollection(x => x.Progressions, p => p.Features))
                ?? throw new DomainValidationException("La definición de clase del personaje está corrupta.");

            int nextLevel = character.Level + 1;
            var nextProgression = classDef.Progressions.FirstOrDefault(p => p.Level == nextLevel);

            bool givesFeatThisLevel = new[] { 4, 8, 12, 16, 19 }.Contains(nextLevel);

            int spellsToLearn = 0;
            var budget = BuildSpellBudget(character, nextProgression);

            if (nextProgression?.Traits != null)
            {
                var nextSpellsTrait = nextProgression.Traits.FirstOrDefault(t =>
                    t.Type.ToString().Contains("PreparedSpells", StringComparison.OrdinalIgnoreCase) ||
                    t.Type.ToString().Contains("Spellcasting", StringComparison.OrdinalIgnoreCase));

                if (nextSpellsTrait != null && int.TryParse(nextSpellsTrait.Value, out int nextMax))
                {
                    spellsToLearn = Math.Max(0, nextMax - character.KnownSpells.Count);
                }
            }
            logger.LogDebug("Borrador creado exitosamente para el personaje {CharacterId}. Nivel objetivo: {TargetLevel}, Dote/ASI: {GivesFeat}", characterId, nextLevel, givesFeatThisLevel);
            return new LevelUpDraft
            {
                CharacterId = characterId,
                TargetLevel = nextLevel,
                GivesFeat = givesFeatThisLevel,
                SpellsToLearnCount = spellsToLearn,
                SpellBudget = budget
            };
        }

        public async Task<LevelUpDraft> PrepareClaimDraftAsync(Guid characterId)
        {
            logger.LogInformation("Preparando reclamación de recompensas pendientes para el personaje: {CharacterId}", characterId);

            var character = await uow.Characters.GetByIdAsync(characterId, config => config.Include(c => c.KnownSpells))
                ?? throw new NotFoundException("Personaje", characterId);

            var classDef = await uow.ClassDefinitions.GetByIdAsync(character.ClassDefId, config => config
                .IncludeCollection(x => x.Progressions, p => p.Features))!;

            var currentProgression = classDef.Progressions.FirstOrDefault(p => p.Level == character.Level);
            var audit = await AuditCharacterAsync(characterId);
            var budget = BuildSpellBudget(character, currentProgression);

            return new LevelUpDraft
            {
                CharacterId = characterId,
                TargetLevel = character.Level,
                HpGain = 0,
                GivesFeat = audit.PendingFeats > 0,
                SpellsToLearnCount = audit.PendingSpells,
                SpellBudget = budget
            };
        }

        // 🚨 HELPER ARQUITECTÓNICO CENTRALIZADO: Mapea cualquier renglón de Excel a un presupuesto tipado
        private SpellBudget BuildSpellBudget(Character character, ClassLevelProgression? progression)
        {
            var budget = new SpellBudget();
            if (character.KnownSpells != null)
            {
                budget.InitiallyKnownSpellIds = character.KnownSpells.Select(s => s.Id).ToList();
                // ❌ Ya no asignamos budget.CurrentSelectionIds aquí porque el estado vive en el LevelUpDraft
            }

            if (progression?.Traits != null)
            {
                // 1. Extraer Trucos oficiales
                var cantripsTrait = progression.Traits.FirstOrDefault(t => t.Type.ToString().Contains("CantripsKnown", StringComparison.OrdinalIgnoreCase));
                if (cantripsTrait != null && int.TryParse(cantripsTrait.Value, out int cMax)) budget.MaxCantrips = cMax;

                // 2. Extraer Hechizos Preparados oficiales
                var preparedTrait = progression.Traits.FirstOrDefault(t => t.Type.ToString().Contains("PreparedSpells", StringComparison.OrdinalIgnoreCase));
                if (preparedTrait != null && int.TryParse(preparedTrait.Value, out int pMax)) budget.MaxPreparedSpells = pMax;

                // 3. Evaluar dinámicamente la matriz de SpellSlots de tu renglón de Excel
                var slotsTrait = progression.Traits.FirstOrDefault(t => t.SpellSlots != null && t.SpellSlots.Any(s => s > 0));
                if (slotsTrait?.SpellSlots != null)
                {
                    for (int i = 0; i < slotsTrait.SpellSlots.Length; i++)
                    {
                        if (slotsTrait.SpellSlots[i] > 0)
                        {
                            budget.MaxSpellLevel = i + 1; // Mapea índice de array a Nivel de Magia real (0=Lvl1, 1=Lvl2...)
                        }
                    }
                }
            }
            return budget;
        }

        public async Task<CharacterDto> CommitLevelUpAsync(LevelUpDraft draft)
        {
            logger.LogInformation("Consolidando subida de nivel para el personaje ID: {CharacterId} hacia el nivel {TargetLevel}", draft.CharacterId, draft.TargetLevel);

            var character = await uow.Characters.GetByIdAsync(draft.CharacterId, config => config
                .Include(c => c.AcquiredFeatures)
                .Include(c => c.CharacterModifiers)
                .Include(c => c.AcquiredFeats)
                .Include(c => c.KnownSpells)
                .Include(c => c.SpellSlots)) 
                ?? throw new NotFoundException("Personaje", draft.CharacterId);

            var classDef = await uow.ClassDefinitions.GetByIdAsync(character.ClassDefId, config => config
                .IncludeCollection(x => x.Progressions, p => p.Features))!;

            // 2. Actualizar vida y nivel si corresponde
            if (character.Level != draft.TargetLevel)
            {
                character.Level = draft.TargetLevel;
                character.MaxHp += draft.HpGain;
                character.CurrentHp = character.MaxHp;
            }

            // 3. Inyectar automáticamente los Features nativos
            var currentProgression = classDef?.Progressions.FirstOrDefault(p => p.Level == character.Level);
            if (currentProgression?.Features != null)
            {
                foreach (var feature in currentProgression.Features)
                {
                    if (!character.AcquiredFeatures.Any(f => f.Id == feature.Id))
                    {
                        uow.SetUnchangedState(feature);
                        character.AcquiredFeatures.Add(feature);
                    }
                }
            }

            // 4. Procesar Incremento de Atributos (ASI)
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
                }
            }

            // 5. Procesar Dote
            if (draft.SelectedFeatId.HasValue)
            {
                var feat = await uow.Feats.GetByIdAsync(draft.SelectedFeatId.Value);
                if (feat != null && !character.AcquiredFeats.Any(f => f.Id == feat.Id))
                {
                    character.AcquiredFeats.Add(feat);
                }
            }

            // 6. Sincronizar Grimorio de Hechizos
            var selectedIds = draft.SelectedSpellIds ?? draft.SelectedSpellIds ?? new List<Guid>();
            var spellsToRemove = character.KnownSpells.Where(s => !selectedIds.Contains(s.Id)).ToList();
            foreach (var spell in spellsToRemove)
            {
                character.KnownSpells.Remove(spell);
            }

            foreach (var spellId in selectedIds)
            {
                if (!character.KnownSpells.Any(s => s.Id == spellId))
                {
                    var spell = await uow.Spells.GetByIdAsync(spellId);
                    if (spell != null)
                    {
                        uow.SetUnchangedState(spell);
                        character.KnownSpells.Add(spell);
                    }
                }
            }

            await uow.SaveChangesAsync();

            await spellService.RecalculateMaxSlotsAsync(character);

            await uow.SaveChangesAsync();
            logger.LogInformation("Nivel consolidado con éxito para el personaje: {CharacterName} ({CharacterId})", character.Name, character.Id);
            return await characterDtoService.ArmDto(character);
        }
        public async Task<CharacterAuditDto> AuditCharacterAsync(Guid characterId)
        {
            logger.LogDebug("Auditando estado del personaje: {CharacterId}", characterId);
            var character = await uow.Characters.GetByIdAsync(characterId, config => config
                .Include(c => c.ClassDef)
                .Include(c => c.AcquiredFeats)
                .Include(c => c.KnownSpells)
                .Include(c => c.CharacterModifiers)
                .IncludePaths.Add("ClassDef.Progressions.Features"))
                ?? throw new NotFoundException("Personaje", characterId);

            var progressions = character.ClassDef.Progressions.Where(x => x.Level <= character.Level).ToList();
            int allowedSpells = 0;
            int expectedFeats = 0;

            var currentProg = progressions.FirstOrDefault(p => p.Level == character.Level);
            if (currentProg?.Traits != null)
            {
                var preparedSpellsTrait = currentProg.Traits.FirstOrDefault(t =>
                    t.Type.ToString().Contains("PreparedSpells", StringComparison.OrdinalIgnoreCase) ||
                    t.Type.ToString().Contains("Spellcasting", StringComparison.OrdinalIgnoreCase));

                if (preparedSpellsTrait != null && int.TryParse(preparedSpellsTrait.Value, out int count))
                {
                    allowedSpells = count;
                }
            }

            foreach (var prog in progressions)
            {
                if (prog.Features != null)
                {
                    bool givesFeatThisLevel = prog.Features.Any(f =>
                        f.RequiresChoice ||
                        f.TechnicalName.Contains("Feat", StringComparison.OrdinalIgnoreCase) ||
                        f.TechnicalName.Contains("AbilityScoreImprovement", StringComparison.OrdinalIgnoreCase) ||
                        f.TechnicalName.Contains("Ability Score Improvement", StringComparison.OrdinalIgnoreCase));

                    if (givesFeatThisLevel) expectedFeats++;
                }
            }

            int pendingSpells = Math.Max(0, allowedSpells - character.KnownSpells.Count);
            int takenFeats = character.AcquiredFeats.Count;
            int takenAsis = character.CharacterModifiers.Count(m => m.Type == ModifierType.AttributeBonus);
            int pendingFeats = Math.Max(0, expectedFeats - (takenFeats + takenAsis));

            return new CharacterAuditDto { PendingFeats = pendingFeats, PendingSpells = pendingSpells };
        }
    }
}