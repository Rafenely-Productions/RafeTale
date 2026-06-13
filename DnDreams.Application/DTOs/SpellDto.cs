using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.DTOs
{
    public class SpellDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MaterialComponentDescription { get; set; } = string.Empty; // Descripción detallada de los componentes materiales, si los hay
        public SpellLevel Level { get; set; } // 0 para Trucos (Cantrips), 1-9 para Conjuros
        public string School { get; set; } = string.Empty; // Evocación, Abjuración, etc.
        public string CastingTime { get; set; } = string.Empty; // 1 Acción, 1 Reacción
        public string Range { get; set; } = string.Empty; // Toque, 120 pies, Personal
        public string RangeDistance { get; set; } = string.Empty;
        public List<string> Components { get; set; } = new(); // V, S, M (Materiales)
        public string Duration { get; set; } = string.Empty; // En turnos, minutos, horas, etc.
        public bool Concentration { get; set; } // "Sí" o "No"
        public bool Ritual { get; set; } // Indica si el hechizo se puede lanzar como ritual
        public List<string> Classes { get; set; } = new();
    }
}
