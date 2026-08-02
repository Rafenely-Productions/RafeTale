using Rafedream.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Domain.Entities
{
    public class SpellSlotLevel : IEntity
    {
        public Guid Id { get; set; }
        public int Level { get; set; }       // Nivel del 1 al 9
        public int MaxSlots { get; set; }    // Ranuras máximas permitidas por nivel/clase
        public int UsedSlots { get; set; }   // Cuántas ha gastado el jugador

        // Propiedad calculada para saber cuántas le quedan
        public int RemainingSlots => Math.Max(0, MaxSlots - UsedSlots);
    }
}
