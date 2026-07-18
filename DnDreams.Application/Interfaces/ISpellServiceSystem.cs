using System;
using System.Threading.Tasks;
using DnDreams.Application.DTOs;

namespace DnDreams.Application.Interfaces
{
    public interface ISpellServiceSystem
    {
        // Consume un Spell Slot del personaje. Soporta Upcasting (lanzar un hechizo de nivel bajo en ranura alta)
        Task<bool> CastSpellAsync(Guid characterId, int spellLevel, int slotLevelToUse);

        // Restaura al 100% todas las ranuras usadas (Regla oficial de Long Rest)
        Task RestRestoreSlotsAsync(Guid characterId);

        // Lógica de automatización: Calcula y sobreescribe cuántas ranuras máximas le tocan al personaje 
        // según su nivel y clase (ideal para invocar tras un CommitLevelUp)
        Task RecalculateMaxSlotsAsync(Guid characterId);
    }
}