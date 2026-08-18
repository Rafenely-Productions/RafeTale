using RafeTale.Application.Interfaces;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using RafeTale.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RafeTale.Application.Services
{
    public class SpellServiceSystem(IUnitOfWork uow) : ISpellServiceSystem
    {
        public async Task<bool> CastSpellAsync(Guid characterId, int spellLevel, int slotLevelToUse)
        {
            var character = await uow.Characters.GetByIdAsync(characterId, q => q.Include(x => x.SpellSlots))
                ?? throw new NotFoundException("Personaje", characterId);

            // Validación de Upcasting: No puedes usar una ranura menor que el nivel base del hechizo
            if (slotLevelToUse < spellLevel) return false;

            // Trucos (Cantrips / Nivel 0): No gastan ranuras //validar esta regla con reglas predefinidas
            if (spellLevel == 0) return true;

            // Buscar la ranura de energía mágica correspondiente
            var targetSlot = character.SpellSlots.FirstOrDefault(s => s.Level == slotLevelToUse);

            // Si el personaje no posee esa ranura o ya se las gastó todas, el lanzamiento falla
            if (targetSlot == null || targetSlot.RemainingSlots <= 0) return false;

            // Gastamos un Pip de energía
            targetSlot.UsedSlots++;

            // Guardamos el cambio en SQLite (EF tracking se encarga)
            await uow.SaveChangesAsync();
            return true;
        }

        public async Task RestRestoreSlotsAsync(Guid characterId)
        {
            var character = await uow.Characters.GetByIdAsync(characterId)
                ?? throw new NotFoundException("Personaje", characterId);

            // Descanso largo: Ponemos los UsedSlots de cada nivel de vuelta en cero
            foreach (var slot in character.SpellSlots)
            {
                slot.UsedSlots = 0;
            }

            await uow.SaveChangesAsync();
        }

        public async Task RecalculateMaxSlotsAsync(Character character)
        {


            if (character.ClassDef == null) return;

            // 2. Buscamos la progresión correspondiente al nivel actual del personaje desde tu repositorio
            var progressions = await uow.ClassLevelProgressions.GetAllAsync(p =>
                p.ClassDefId == character.ClassDefId && p.Level == character.Level);

            var currentProg = progressions.FirstOrDefault();

            // Si no hay datos de progresión o tu lista de rasgos de clase viene vacía, salimos de forma segura
            if (currentProg == null || currentProg.Traits == null || currentProg.Traits.Count == 0)
            {
                return;
            }

            // 3. OBTENEMOS EL RASGO DE LANZAMIENTO DE CONJUROS (Spellcasting)
            // Buscamos dentro de tus ClassTraits aquel que contenga el array configurado
            var spellcastingTrait = currentProg.Traits.FirstOrDefault(t => t.SpellSlots != null && t.SpellSlots.Any(slots => slots > 0));

            if (spellcastingTrait == null) return; // Si este nivel no otorga slots (ej: un nivel de Guerrero puro), salimos

            var activeLevels = new List<int>();

            // 4. MAPEO DIRECTO DESDE TU ARRAY REAL 'int[] SpellSlots'
            for (int i = 0; i < spellcastingTrait.SpellSlots.Length; i++)
            {
                int level = i + 1;
                int maxSlotsFromDb = spellcastingTrait.SpellSlots[i];

                if (maxSlotsFromDb > 0)
                {
                    activeLevels.Add(level);

                    var existing = character.SpellSlots.FirstOrDefault(s => s.Level == level);

                    // 🌟 COMPROBACIÓN ANTIBOMBAS USANDO TU UOW:
                    bool existsInDatabase = false;
                    if (existing != null)
                    {
                        // Le preguntamos a la infraestructura si el Id realmente existe en SQLite
                        existsInDatabase = await uow.SpellSlotExistsAsync(existing.Id);
                    }

                    if (existing != null && existsInDatabase)
                    {
                        // ACTUALIZACIÓN IN-PLACE LEGÍTIMA: El registro existe en disco, EF puede hacer UPDATE seguro
                        existing.MaxSlots = maxSlotsFromDb;
                        existing.UsedSlots = Math.Min(existing.UsedSlots, maxSlotsFromDb);
                    }
                    else
                    {
                        // INSERCIÓN LIMPIA (ADDED): Si no existía en disco, limpiamos el rastreador suco
                        // y lo forzamos a entrar como un registro totalmente nuevo (State: Added)
                        if (existing != null)
                        {
                            character.SpellSlots.Remove(existing);
                        }
                        var newSlot = new CharacterSpellSlots
                        {
                            Id = Guid.NewGuid(),
                            CharacterId = character.Id,
                            Level = level,
                            MaxSlots = maxSlotsFromDb,
                            UsedSlots = 0
                        };
                        uow.TrackNewSpellSlot(newSlot);
                        character.SpellSlots.Add(newSlot);
                    }
                }
            }
            var slotsToRemove = character.SpellSlots.Where(s => !activeLevels.Contains(s.Level)).ToList();
            foreach (var oldSlot in slotsToRemove)
            {
                character.SpellSlots.Remove(oldSlot);
            }
        }
    }
}