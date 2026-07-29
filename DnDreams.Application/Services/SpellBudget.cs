using DnDreams.Application.DTOs;

namespace DnDreams.Application.Services
{
    public class SpellBudget
    {
        public int MaxCantrips { get; set; }
        public int MaxPreparedSpells { get; set; }
        public int MaxSpellLevel { get; set; }

        // Hechizos que el personaje ya tenía antes de entrar a este flujo
        public List<Guid> InitiallyKnownSpellIds { get; set; } = new();

        // El estado actual de la selección en la UI (empieza clonando InitiallyKnownSpellIds)
        public List<Guid> CurrentSelectionIds { get; set; } = new();

        // Lógica dinámica reutilizable para la UI o el Backend
        public int SelectedCantripsCount(List<SpellDto> allSpells) =>
            allSpells.Count(s => CurrentSelectionIds.Contains(s.Id) && (int)s.Level == 0);

        public int SelectedSpellsCount(List<SpellDto> allSpells) =>
            allSpells.Count(s => CurrentSelectionIds.Contains(s.Id) && (int)s.Level > 0);

        // Validación inteligente: ¿Es una selección legal según las reglas de D&D 2024?
        public (bool IsValid, string ValidationError) Validate(List<SpellDto> allSpells)
        {
            var selectedSpells = allSpells.Where(s => CurrentSelectionIds.Contains(s.Id)).ToList();

            int cantripsCount = selectedSpells.Count(s => (int)s.Level == 0);
            int spellsCount = selectedSpells.Count(s => (int)s.Level > 0);

            if (cantripsCount > MaxCantrips)
                return (false, $"Has superado el límite de trucos (Cantrips) permitidos ({MaxCantrips}).");

            if (spellsCount > MaxPreparedSpells)
                return (false, $"Has superado el límite de conjuros preparados ({MaxPreparedSpells}).");

            if (selectedSpells.Any(s => (int)s.Level > MaxSpellLevel))
                return (false, $"Seleccionaste un hechizo de nivel superior al permitido (Máximo Nivel {MaxSpellLevel}).");

            return (true, string.Empty);
        }
    }
}
