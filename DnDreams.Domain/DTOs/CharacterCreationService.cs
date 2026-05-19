using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.DTOs
{
    public class CharacterCreationService
    {
        // Aquí guardamos lo que el usuario va eligiendo
        public Guid? SelectedRaceId { get; set; }
        public Guid? SelectedClassId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;

        public Dictionary<ASI, int> BaseStats { get; set; } = new()
        {
            { ASI.Strength, 10 }, { ASI.Dexterity, 10 }, { ASI.Constitution, 10 },
            { ASI.Intelligence, 10 }, { ASI.Wisdom, 10 }, { ASI.Charisma, 10 }
        };

        public void Reset()
        {
            SelectedRaceId = null;
            SelectedClassId = null;
            Name = string.Empty;
            History = string.Empty;
        }
    }
}
