using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;

namespace DnDreams.Application.DTOs
{
    public class CharacterDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string History { get; init; } = string.Empty;

        // Atributos Calculados (Ya procesados con modificadores)
        public int Strength { get; init; }
        public int Dexterity { get; init; }
        public int Constitution { get; init; }
        public int Intelligence { get; init; }
        public int Wisdom { get; init; }
        public int Charisma { get; init; }

        // Modificadores directos para la UI
        public int StrModifier => (int)Math.Floor((Strength - 10) / 2.0);
        public int DexModifier => (int)Math.Floor((Dexterity - 10) / 2.0);
        public int ConModifier => (int)Math.Floor((Constitution - 10) / 2.0);
        public int IntModifier => (int)Math.Floor((Intelligence - 10) / 2.0);
        public int WisModifier => (int)Math.Floor((Wisdom - 10) / 2.0);
        public int ChaModifier => (int)Math.Floor((Charisma - 10) / 2.0);

        // Datos vitales
        public int MaxHp { get; init; }
        public int CurrentHp { get; init; }
        public int Level { get; init; } = 1;
        public int Experience { get; init; } = 0;
        public int ProficiencyBonus => 2 + ((Level - 1) / 4);

        // Relaciones resueltas a DTOs
        public RaceDto? Race { get; init; }
        public ClassDefinitionDto? ClassDef { get; init; }
        public BackgroundDto? Background { get; init; }
        public List<CharacterSpellSlots> SpellSlots { get; init; } = new();
        // Colecciones de rasgos e idiomas ya traducidos
        public List<FeatureDto> AcquiredFeatures { get; init; } = new();
        public List<FeatDto> AcquiredFeats { get; init; } = new();

        // Método calculador de habilidades para el DTO
        public int GetSkillBonus(string skillKey, string baseStat, List<string> trainedSkills)
        {
            int statMod = baseStat switch
            {
                "Strength" => StrModifier,
                "Dexterity" => DexModifier,
                "Constitution" => ConModifier,
                "Intelligence" => IntModifier,
                "Wisdom" => WisModifier,
                "Charisma" => ChaModifier,
                _ => 0
            };

            bool isProficient = trainedSkills.Contains(skillKey);
            return statMod + (isProficient ? ProficiencyBonus : 0);
        }
    }
}