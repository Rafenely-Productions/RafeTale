using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.DTOs
{
    public class CharacterCreationService
    {
        // Aquí guardamos lo que el usuario va eligiendo
        public Guid? SelectedRaceId { get; set; }
        public Guid? SelectedClassId { get; set; }
        public Guid? SelectedFeatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;

        public Dictionary<ASI, int> BaseStats { get; set; } = new()
        {
            { ASI.Strength, 10 }, { ASI.Dexterity, 10 }, { ASI.Constitution, 10 },
            { ASI.Intelligence, 10 }, { ASI.Wisdom, 10 }, { ASI.Charisma, 10 }
        };

        public void Reset()
        {
            SelectedRaceId = null;
            SelectedClassId = null;
            Name = string.Empty;
            History = string.Empty;
        }

        private async Task HandleCreate()
        {
            /*try
            {
                // 1. Obtener los datos completos de Raza y Clase
                var race = await UnitOfWork.Races.GetByIdAsync(Model.SelectedRaceId);
                var classDef = await UnitOfWork.ClassDefinitions.GetByIdAsync(Model.SelectedClassId, x => x.Progressions);

                // 2. Crear la entidad Character
                var newHero = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = Model.Name,
                    History = Model.History,
                    RaceId = race.Id,
                    ClassDefinitionId = classDef.Id,
                    Level = 1,
                    Experience = 0,

                    // HP Inicial: Dado de vida de la clase + modificador de CON (ejemplo simplificado)
                    MaxHp = classDef.HitDie + 2,
                    CurrentHp = classDef.HitDie + 2,

                    // Atributos base (puedes inicializarlos en 10 o según tu lógica)
                    Strength = 10,
                    Dexterity = 10, // ... etc
                };

                // 3. Aplicar "Features" de Nivel 1
                var level1Progression = classDef.Progressions.FirstOrDefault(p => p.Level == 1);
                if (level1Progression != null)
                {
                    // Aquí vincularías las habilidades iniciales al personaje
                    // newHero.Features.AddRange(level1Progression.Features);
                }

                // 4. Guardar
                await UnitOfWork.Characters.AddAsync(newHero);
                await UnitOfWork.SaveChangesAsync();

                // 5. ¡A la aventura!
                Navigation.NavigateTo($"/character/sheet/{newHero.Id}");
            }
            catch (Exception ex)
            {
                // Notificar error al usuario
            }*/
        }
    }
}
