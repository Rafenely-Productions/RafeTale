using RafeTale.Application.Interfaces.DtosInterfaces;

namespace RafeTale.Application.DTOs
{
    public class SpellDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // Nombre técnico para referencias internas
        public string TechnicalName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MaterialComponentDescription { get; set; } = string.Empty; // Descripción detallada de los componentes materiales, si los hay
        public int Level { get; set; } // 0 para Trucos (Cantrips), 1-9 para Conjuros
        public string School { get; set; } = string.Empty;// Evocación, Abjuración, etc.
        public string CastingTime { get; set; } = string.Empty; // 1 Acción, 1 Reacción
        public string Range { get; set; } = string.Empty; // Toque, 120 pies, Personal
        public string RangeDistance { get; set; } = string.Empty;
        public List<string> Components { get; set; } = []; // V, S, M (Materiales)
        public List<string> Duration { get; set; } = []; // En turnos, minutos, horas, etc.
        public bool Concentration { get; set; } // "Sí" o "No"
        public bool Ritual { get; set; } // Indica si el hechizo se puede lanzar como ritual
        public List<string> ClassesTechnicalNames { get; set; } = [];
    }
}
