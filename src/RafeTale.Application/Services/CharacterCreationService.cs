using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Exceptions;
using RafeTale.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RafeTale.Application.Services
{
    public class CharacterCreationService : ICharacterCreationService
    {
        // Estado temporal de creación
        public Guid? SelectedRaceId { get; set; }
        public Guid? SelectedClassId { get; set; }
        public Guid? SelectedBackgroundId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;

        public Dictionary<AttributeImprovementChoice, int> BaseStats { get; set; } = new()
        {
            { AttributeImprovementChoice.Strength, 10 }, { AttributeImprovementChoice.Dexterity, 10 }, { AttributeImprovementChoice.Constitution, 10 },
            { AttributeImprovementChoice.Intelligence, 10 }, { AttributeImprovementChoice.Wisdom, 10 }, { AttributeImprovementChoice.Charisma, 10 }
        };

        public Dictionary<AttributeImprovementChoice, int> BonusStats { get; set; } = new()
        {
            { AttributeImprovementChoice.Strength, 0 }, { AttributeImprovementChoice.Dexterity, 0 }, { AttributeImprovementChoice.Constitution, 0 },
            { AttributeImprovementChoice.Intelligence, 0 }, { AttributeImprovementChoice.Wisdom, 0 }, { AttributeImprovementChoice.Charisma, 0 }
        };

        public void Reset()
        {
            SelectedRaceId = null;
            SelectedClassId = null;
            SelectedBackgroundId = null;
            Name = string.Empty;
            History = string.Empty;

            foreach (var stat in Enum.GetValues<AttributeImprovementChoice>())
            {
                BaseStats[stat] = 10;
                BonusStats[stat] = 0;
            }
        }

        // Método estrella para persistir de verdad el nuevo héroe en la DB
        public async Task<Character> CreateAndSaveCharacterAsync(IUnitOfWork uow)
        {
            if (!SelectedRaceId.HasValue || !SelectedClassId.HasValue || !SelectedBackgroundId.HasValue)
            {
                throw new DomainValidationException("No se puede crear el personaje porque faltan selecciones obligatorias.");
            }

            // 1. Cargar las definiciones de raza, clase y trasfondo de la base de datos
            var race = await uow.Races.GetByIdAsync(SelectedRaceId.Value)
                ?? throw new NotFoundException("Raza", SelectedRaceId.Value);

            var classDef = await uow.ClassDefinitions.GetByIdAsync(SelectedClassId.Value)
                ?? throw new NotFoundException("Clase", SelectedClassId.Value);

            var background = await uow.Backgrounds.GetByIdAsync(SelectedBackgroundId.Value)
                ?? throw new NotFoundException("Trasfondo", SelectedBackgroundId.Value);

            // 2. Calcular estadísticas finales (Base + Bono de Trasfondo)
            int finalCon = BaseStats[AttributeImprovementChoice.Constitution] + BonusStats[AttributeImprovementChoice.Constitution];
            int conModifier = (int)Math.Floor((finalCon - 10) / 2.0);

            // 3. HP Inicial según reglas 2024: Dado de vida máximo al nivel 1 + modificador de Constitución
            int startingHp = classDef.HitDieValue + conModifier;

            // 4. Crear la entidad real de Dominio
            var newHero = new Character
            {
                Id = Guid.NewGuid(),
                Name = Name,
                History = History,
                RaceId = race.Id,
                ClassDefId = classDef.Id,
                BackgroundId = background.Id,
                Level = 1,
                Experience = 0,

                MaxHp = startingHp,
                CurrentHp = startingHp,

                Stats = new Dictionary<string, int>
                {
                    { TargetPropertyType.Strength.ToString(), BaseStats[AttributeImprovementChoice.Strength] },
                    { TargetPropertyType.Dexterity.ToString(), BaseStats[AttributeImprovementChoice.Dexterity] },
                    { TargetPropertyType.Constitution.ToString(), BaseStats[AttributeImprovementChoice.Constitution] },
                    { TargetPropertyType.Intelligence.ToString(), BaseStats[AttributeImprovementChoice.Intelligence] },
                    { TargetPropertyType.Wisdom.ToString(), BaseStats[AttributeImprovementChoice.Wisdom] },
                    { TargetPropertyType.Charisma.ToString(), BaseStats[AttributeImprovementChoice.Charisma] }
                }
            };
            foreach (var stat in Enum.GetValues<AttributeImprovementChoice>())
            {
                if (BonusStats[stat] > 0)
                {
                    newHero.CharacterModifiers.Add(new CharacterModifier
                    {
                        Id = Guid.NewGuid(),
                        CharacterId = newHero.Id,
                        Type = ModifierType.AttributeBonus,
                        Target = stat.ToString(), // Coincide con tu TargetPropertyType
                        Value = BonusStats[stat]
                    });
                }
            }
            // 5. Guardar en SQLite
            await uow.Characters.AddAsync(newHero);
            await uow.SaveChangesAsync();

            // Limpiamos el draft temporal del servicio para el próximo personaje
            Reset();

            return newHero;
        }
    }
}