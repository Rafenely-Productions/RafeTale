using ClosedXML.Excel;
using DnDreams.Application.Interfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DnDreams.Application.Services;

public class ImportManager : IExcelImportService
{
    private readonly IUnitOfWork _unitOfWork;

    // Inyectamos el orquestador maestro de transacciones
    public ImportManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(int Count, string Version)> ImportDataFromExcelAsync(Stream excelStream)
    {
        var racesList = new List<Race>();
        var classesList = new List<ClassDefinition>();
        var charactersList = new List<Character>();
        var progressionsList = new List<ClassLevelProgression>();

        using var workbook = new XLWorkbook(excelStream);

        // 1. LEER PESTAÑA DE RAZAS
        if (workbook.TryGetWorksheet("Razas", out var raceSheet))
        {
            var rows = raceSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var race = new Race
                {
                    Id = Guid.NewGuid(),
                    Name = row.Cell(1).GetString(),
                    Speed = row.Cell(2).GetValue<int>()
                };

                // Leer columnas de bonos dinámicos (Fuerza, Destreza, etc.)
                for (int col = 3; col <= raceSheet.LastColumnUsed().ColumnNumber(); col++)
                {
                    var statName = raceSheet.Cell(1, col).GetString();
                    var statValue = row.Cell(col).GetValue<int>();
                    if (!string.IsNullOrEmpty(statName) && statValue != 0)
                    {
                        race.StatBonuses[statName] = statValue;
                    }
                }
                racesList.Add(race);
            }
        }

        // 2. LEER PESTAÑA DE CLASES
        if (workbook.TryGetWorksheet("Clases", out var classSheet))
        {
            var rows = classSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                classesList.Add(new ClassDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = row.Cell(1).GetString(),
                    HitDie = row.Cell(2).GetString()
                });
            }
        }

        // 3. LEER PESTAÑA DE PERSONAJES
        if (workbook.TryGetWorksheet("Personajes", out var charSheet))
        {
            var rows = charSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var charName = row.Cell(1).GetString();
                var raceName = row.Cell(2).GetString();
                var className = row.Cell(3).GetString();

                // Buscamos temporalmente en las listas de arriba para relacionar los IDs mockeados
                var matchedRace = racesList.FirstOrDefault(r => r.Name.Equals(raceName, StringComparison.OrdinalIgnoreCase));
                var matchedClass = classesList.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = charName,
                    Level = row.Cell(4).GetValue<int>(),
                    Experience = row.Cell(5).GetValue<int>(),
                    RaceId = matchedRace?.Id ?? Guid.Empty,
                    ClassDefId = matchedClass?.Id ?? Guid.Empty
                };

                // Mapear los Stats base al diccionario dinámico
                for (int col = 6; col <= charSheet.LastColumnUsed().ColumnNumber(); col++)
                {
                    var statName = charSheet.Cell(1, col).GetString();
                    var statValue = row.Cell(col).GetValue<int>();
                    if (!string.IsNullOrEmpty(statName))
                    {
                        character.Stats[statName] = statValue;
                    }
                }
                charactersList.Add(character);
            }
        }

        // 4. LEER PESTAÑA DE ProgresoClases
        if (workbook.TryGetWorksheet("ProgresoClases", out var progressSheet))
        {
            var progressRows = progressSheet.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in progressRows)
            {
                var className = row.Cell(1).GetString().Trim();
                var level = row.Cell(2).GetValue<int>();
                var featureName = row.Cell(3).GetString().Trim();
                var featureDescription = row.Cell(4).GetString().Trim() ?? $"Rasgo de nivel {level}";

                if (string.IsNullOrEmpty(featureDescription))
                {
                    featureDescription = $"Rasgo de nivel {level}";
                }
                var modifiersJson = row.Cell(5).GetString() is string json && !string.IsNullOrWhiteSpace(json) ? json : "[]";
                
                if (string.IsNullOrEmpty(featureName)) continue;

                var targetClass = classesList.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

                if (targetClass == null) continue;

                var feature = new Feature
                {
                    Id = Guid.NewGuid(),
                    Name = featureName,
                    Description = featureDescription,
                    RequiresChoice = featureName.Contains("Elegir", StringComparison.OrdinalIgnoreCase) ||
                         featureName.Contains("Arquetipo", StringComparison.OrdinalIgnoreCase),
                    ModifiersJson = modifiersJson
                };
                var existingProgression = progressionsList.FirstOrDefault(p => p.ClassDefId == targetClass.Id && p.Level == level);

                if (existingProgression != null)
                {
                    // Si la progresión de ese nivel ya existía, solo le inyectamos el nuevo rasgo a su colección
                    existingProgression.Features.Add(feature);
                }
                else
                {
                    // Si es la primera vez que pasamos por este nivel, creamos la progresión base
                    var newProgression = new ClassLevelProgression
                    {
                        Id = Guid.NewGuid(),
                        Level = level,
                        ClassDefId = targetClass.Id,
                        Features = new List<Feature> { feature } // <-- Metemos el Feature real con sus datos
                    };

                    progressionsList.Add(newProgression);
                }
            }
        }

        var xpRulesList = new List<XpRules>();
        if (workbook.TryGetWorksheet("ReglasXP", out var xpSheet))
        {
            var rows = xpSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                xpRulesList.Add(new XpRules
                {
                    Level = row.Cell(1).GetValue<int>(), // Usamos el nivel como ID o mapeamos el nivel
                    RequiredXp = row.Cell(2).GetValue<int>(),
                    Bonus = xpSheet.LastColumnUsed().ColumnNumber() >= 3 ? row.Cell(3).GetValue<int>() : 0
                });
            }
        }

        var featsList = new List<Feat>();
        if (workbook.TryGetWorksheet("Dotes", out var featSheet))
        {
            var rows = featSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                featsList.Add(new Feat
                {
                    Id = Guid.NewGuid(),
                    Name = row.Cell(1).GetString() ?? string.Empty,
                    Description = row.Cell(2).GetString() ?? string.Empty,
                    Prerequisite = row.Cell(3).GetString() ?? "Ninguno",
                    ModifiersJson = row.Cell(4).GetString() is string json && !string.IsNullOrWhiteSpace(json)
                        ? json
                        : "[]"
                });
            }
        }

        var spellsList = new List<Spell>();
        if (workbook.TryGetWorksheet("Hechizos", out var spellSheet))
        {
            var rows = spellSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                spellsList.Add(new Spell
                {
                    Id = Guid.NewGuid(),
                    Name = row.Cell(1).GetString() ?? string.Empty,
                    Level = row.Cell(2).GetValue<int>(),
                    School = row.Cell(3).GetString() ?? string.Empty,
                    CastingTime = row.Cell(4).GetString() ?? string.Empty,
                    Range = row.Cell(5).GetString() ?? string.Empty,
                    Description = row.Cell(6).GetString() ?? string.Empty
                });
            }
        }

        // GUARDADO DE DATOS CON UNIT OF WORK
        // ==========================================
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Mandamos los datos maestros a sus respectivos mini-repositorios
            await _unitOfWork.Races.AddRangeAsync(racesList);
            await _unitOfWork.Classes.AddRangeAsync(classesList);
            await _unitOfWork.Classes.AddProgressionsRangeAsync(progressionsList);
            await _unitOfWork.XpRules.AddRangeAsync(xpRulesList);
            await _unitOfWork.Feats.AddRangeAsync(featsList);
            await _unitOfWork.Spells.AddRangeAsync(spellsList);

            // Hacemos un save intermedio para consolidar llaves primarias en la memoria de EF
            await _unitOfWork.SaveChangesAsync();

            // Mapeamos los IDs reales a los personajes antes de insertarlos (Garantiza las FKs)
            foreach (var character in charactersList)
            {
                var targetRaceName = racesList.FirstOrDefault(r => r.Id == character.RaceId)?.Name ?? string.Empty;
                var targetClassName = classesList.FirstOrDefault(c => c.Id == character.ClassDefId)?.Name ?? string.Empty;

                var dbRace = await _unitOfWork.Races.GetByNameAsync(targetRaceName);
                var dbClass = await _unitOfWork.Classes.GetByNameAsync(targetClassName);

                character.RaceId = dbRace?.Id ?? character.RaceId;
                character.ClassDefId = dbClass?.Id ?? character.ClassDefId;

                if (dbClass != null && character.Level > 0)
                {
                    // Buscamos todas las progresiones de esta clase que correspondan a los niveles 
                    // que el personaje YA TIENE (desde nivel 1 hasta su nivel actual del Excel)
                    var earnedProgressions = progressionsList
                        .Where(p => p.ClassDefId == dbClass.Id && p.Level <= character.Level)
                        .ToList();

                    foreach (var prog in earnedProgressions)
                    {
                        if (prog.Features != null && prog.Features.Any())
                        {
                            foreach (var feature in prog.Features)
                            {
                                // Evitamos duplicaciones y le asignamos el rasgo al personaje
                                if (!character.AcquiredFeatures.Any(f => f.Id == feature.Id))
                                {
                                    character.AcquiredFeatures.Add(feature);
                                }
                            }
                        }
                    }
                }
            }

            await _unitOfWork.Characters.AddRangeAsync(charactersList);

            // Guardado definitivo y consolidación de la transacción en el archivo .db3
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return (Count: charactersList.Count, Version: "1.0");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}