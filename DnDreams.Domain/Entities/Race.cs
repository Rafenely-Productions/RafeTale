using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Entities
{
    public class Race
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public int Speed { get; set; } = 30;
        public Dictionary<string, int> StatBonuses { get; set; } = new(); // <- JSON en BD
    }
}
