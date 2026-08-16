using RafeTale.Application.Services;

namespace RafeTale.Application.DTOs
{
    public class LevelUpDraft
    {
        public Guid CharacterId { get; init; }
        public int TargetLevel { get; set; }

        // Puntos de vida (HP) obtenidos en este nivel
        public int HpGain { get; set; }
        public bool UseAverageHp { get; set; } = true;

        // Dotes / Incremento de Atributo (ASI)
        public bool GivesFeat { get; init; }
        public Guid? SelectedFeatId { get; set; }

        public string? SelectedAsiOne { get; set; }
        public string? SelectedAsiTwo { get; set; }

        // Hechizos / Spells
        public int SpellsToLearnCount { get; init; }
        public List<Guid> SelectedSpellIds { get; set; } = new();

        public SpellBudget SpellBudget { get; set; } = new();

    }
}