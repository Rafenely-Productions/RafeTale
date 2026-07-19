using DnDreams.Application.Services;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;

namespace DnDreams.Application.DTOs
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

        // Si elige subir Atributos Puro en lugar de Dote (D&D 2024 permite +2 o +1/+1)
        public ASI? SelectedAsiOne { get; set; }
        public ASI? SelectedAsiTwo { get; set; }

        // Hechizos / Spells
        public int SpellsToLearnCount { get; init; }
        public List<Guid> SelectedSpellIds { get; set; } = new();

        public SpellBudget SpellBudget { get; set; } = new();

    }
}