using Rafedream.Application.Services;
using Rafedream.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Rafedream.Application.DTOs
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

        public ASI? SelectedAsiOne { get; set; }
        public ASI? SelectedAsiTwo { get; set; }

        // Hechizos / Spells
        public int SpellsToLearnCount { get; init; }
        public List<Guid> SelectedSpellIds { get; set; } = new();

        public SpellBudget SpellBudget { get; set; } = new();

    }
}