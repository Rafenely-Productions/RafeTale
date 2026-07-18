using DnDreams.Application.Interfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DnDreams.Application.Services
{
    public class SpellServiceSystem(IUnitOfWork uow) : ISpellServiceSystem
    {
        public async Task<bool> CastSpellAsync(Guid characterId, int spellLevel, int slotLevelToUse)
        {
            var character = await uow.Characters.GetByIdAsync(characterId)
                ?? throw new Exception("Personaje ausente del plano material.");

            // Validación de Upcasting: No puedes usar una ranura menor que el nivel base del hechizo
            if (slotLevelToUse < spellLevel) return false;

            // Trucos (Cantrips / Nivel 0): No gastan ranuras en D&D 2024
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
                ?? throw new Exception("Personaje ausente.");

            // Descanso largo: Ponemos los UsedSlots de cada nivel de vuelta en cero
            foreach (var slot in character.SpellSlots)
            {
                slot.UsedSlots = 0;
            }

            await uow.SaveChangesAsync();
        }

        public async Task RecalculateMaxSlotsAsync(Guid characterId)
        {
            // 1. Cargamos el personaje incluyendo sus ranuras físicas relacionales actuales
            var character = await uow.Characters.GetByIdAsync(characterId, config => config
                .Include(c => c.ClassDef)
                .Include(c => c.SpellSlots))
                ?? throw new Exception("Personaje ausente.");

            if (character.ClassDef == null) return;

            // 2. Buscamos la progresión correspondiente al nivel actual del personaje desde tu repositorio
            var progressions = await uow.ClassLevelProgressions.GetAllAsync(p =>
                p.ClassDefId == character.ClassDefId && p.Level == character.Level);

            var currentProg = progressions.FirstOrDefault();

            // Si no hay datos de progresión o tu lista de rasgos de clase viene vacía, salimos de forma segura
            if (currentProg == null || currentProg.Traits == null || !currentProg.Traits.Any())
            {
                return;
            }

            // 3. OBTENEMOS EL RASGO DE LANZAMIENTO DE CONJUROS (Spellcasting)
            // Buscamos dentro de tus ClassTraits aquel que contenga el array configurado
            var spellcastingTrait = currentProg.Traits.FirstOrDefault(t => t.SpellSlots != null && t.SpellSlots.Any(slots => slots > 0));

            if (spellcastingTrait == null) return; // Si este nivel no otorga slots (ej: un nivel de Guerrero puro), salimos

            var newSlots = new List<CharacterSpellSlots>();

            // 4. MAPEO DIRECTO DESDE TU ARRAY REAL 'int[] SpellSlots'
            for (int i = 0; i < spellcastingTrait.SpellSlots.Length; i++)
            {
                int level = i + 1;
                int maxSlotsFromDb = spellcastingTrait.SpellSlots[i];

                // Si el nivel de clase otorga ranuras para este círculo de conjuro...
                if (maxSlotsFromDb > 0)
                {
                    // Buscamos si el personaje ya tenía esta burbuja guardada en SQLite para mantener sus UsedSlots intactos
                    var existing = character.SpellSlots.FirstOrDefault(s => s.Level == level);

                    newSlots.Add(new CharacterSpellSlots
                    {
                        Id = existing?.Id ?? Guid.NewGuid(),
                        CharacterId = character.Id,
                        Level = level,
                        MaxSlots = maxSlotsFromDb,
                        // Protegemos que los slots usados no queden flotando por encima del máximo permitido
                        UsedSlots = existing != null ? Math.Min(existing.UsedSlots, maxSlotsFromDb) : 0
                    });
                }
            }

            // 5. Sincronización y persistencia física relacional directa en la tabla SQLite
            character.SpellSlots = newSlots;
            await uow.SaveChangesAsync();
        }
    }
}