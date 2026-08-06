using RafeTale.Application.DTOs;

namespace RafeTale.Application.Services;

public class SpellBudget
{
    public int MaxCantrips { get; set; }
    public int MaxPreparedSpells { get; set; }
    public int MaxSpellLevel { get; set; }

    // Hechizos que el personaje ya tenía antes de entrar a este flujo (baseline)
    public List<Guid> InitiallyKnownSpellIds { get; set; } = new();

    // Lógica dinámica reutilizable, ahora recibe la selección actual como parámetro
    public int SelectedCantripsCount(IEnumerable<Guid> selectedIds, List<SpellDto> allSpells) =>
        allSpells.Count(s => selectedIds.Contains(s.Id) && (int)s.Level == 0);

    public int SelectedSpellsCount(IEnumerable<Guid> selectedIds, List<SpellDto> allSpells) =>
        allSpells.Count(s => selectedIds.Contains(s.Id) && (int)s.Level > 0);

    // Validación pura: No guarda estado, solo calcula. Devuelve llaves de error para i18n.
    public (bool IsValid, string ErrorKey) Validate(IEnumerable<Guid> selectedIds, List<SpellDto> allSpells)
    {
        var selectedSpells = allSpells.Where(s => selectedIds.Contains(s.Id)).ToList();

        int cantripsCount = selectedSpells.Count(s => (int)s.Level == 0);
        int spellsCount = selectedSpells.Count(s => (int)s.Level > 0);

        if (cantripsCount > MaxCantrips)
            return (false, "Error_MaxCantripsExceeded");

        if (spellsCount > MaxPreparedSpells)
            return (false, "Error_MaxSpellsExceeded");

        if (selectedSpells.Any(s => (int)s.Level > MaxSpellLevel))
            return (false, "Error_MaxSpellLevelExceeded");

        return (true, string.Empty);
    }
}