using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DnDreams.Domain.Modifiers;

namespace DnDreams.Domain.Entities
{
    public class Feature
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Indica si el jugador debe elegir algo (ej: un hechizo o subclase)
        public bool RequiresChoice { get; set; } = false;

        public string ModifiersJson { get; set; } = "[]";

        // Propiedad calculada ignorada por EF Core para usar en el motor
        public List<ModifierData> Modifiers
        {
            get
            {
                try
                {
                    return JsonSerializer.Deserialize<List<ModifierData>>(ModifiersJson) ?? new List<ModifierData>();
                }
                catch
                {
                    return new List<ModifierData>();
                }
            }
        }
    }
}
