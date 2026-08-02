using Rafedream.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Application.Interfaces
{
    public interface ILevelUpService
    {
        // Evalúa la progresión de la clase y te dice exactamente qué necesita elegir el usuario
        Task<LevelUpDraft> PrepareLevelUpAsync(Guid characterId);

        // Aplica las elecciones del Draft, inyecta los nuevos Features/Spells/Stats al Character y guarda en DB
        Task<CharacterDto> CommitLevelUpAsync(LevelUpDraft draft);
        Task<CharacterAuditDto> AuditCharacterAsync(Guid characterId);
        Task<LevelUpDraft> PrepareClaimDraftAsync(Guid characterId);


    }
    public class CharacterAuditDto
    {
        public int PendingFeats { get; set; }
        public int PendingSpells { get; set; }
        public bool HasPendingChoices => PendingFeats > 0 || PendingSpells > 0;
    }
}
