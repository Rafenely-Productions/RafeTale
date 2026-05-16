using ClosedXML.Excel;
using DnDreams.Application.Interfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DnDreams.Application.Services;

public class ImportManager : IExcelImportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ImportManager(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(int Count, string Version)> ImportDataFromExcelAsync(Stream excelStream)
    {
        var racesList = new List<Race>();
        var classDefinitionList = new List<ClassDefinition>();
        var charactersList = new List<Character>();
        var progressionsList = new List<ClassLevelProgression>();
        var spellsList = new List<Spell>();
        var xpRulesList = new List<XpRules>();
        var featsList = new List<Feat>();
        var itemsList = new List<ItemTemplate>();

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
                classDefinitionList.Add(new ClassDefinition
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

                var matchedRace = racesList.FirstOrDefault(r => r.Name.Equals(raceName, StringComparison.OrdinalIgnoreCase));
                var matchedClass = classDefinitionList.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = charName,
                    Level = row.Cell(4).GetValue<int>(),
                    Experience = row.Cell(5).GetValue<int>(),
                    RaceId = matchedRace?.Id ?? Guid.Empty,
                    ClassDefId = matchedClass?.Id ?? Guid.Empty
                };

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
                var modifiersJson = row.Cell(5).GetString() is string json && !string.IsNullOrWhiteSpace(json) ? json : "[]";

                if (string.IsNullOrEmpty(featureName)) continue;

                var targetClass = classDefinitionList.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

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
                    existingProgression.Features.Add(feature);
                }
                else
                {
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

        // LEER REGLAS XP, DOTES, HECHIZOS E ITEMS
        if (workbook.TryGetWorksheet("ReglasXP", out var xpSheet))
        {
            var rows = xpSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                xpRulesList.Add(new XpRules
                {
                    Level = row.Cell(1).GetValue<int>(),
                    RequiredXp = row.Cell(2).GetValue<int>(),
                    Bonus = xpSheet.LastColumnUsed().ColumnNumber() >= 3 ? row.Cell(3).GetValue<int>() : 0
                });
            }
        }

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
                    ModifiersJson = row.Cell(4).GetString() is string json && !string.IsNullOrWhiteSpace(json) ? json : "[]"
                });
            }
        }

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

        if (workbook.TryGetWorksheet("Items", out var itemsSheet))
        {
            var rows = itemsSheet.RangeUsed().RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var itemName = row.Cell(1).GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(itemName)) continue;

                var template = new ItemTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = itemName,
                    Description = row.Cell(2).GetString() ?? string.Empty,
                    Category = itemsSheet.LastColumnUsed().ColumnNumber() >= 3
                        ? (ItemCategory)Enum.Parse(typeof(ItemCategory), row.Cell(3).GetString(), true)
                        : ItemCategory.AdventuringGear
                };
                itemsList.Add(template);

                if (itemsSheet.LastColumnUsed().ColumnNumber() >= 4)
                {
                    var ownerName = row.Cell(4).GetString();
                    var matchedChar = charactersList.FirstOrDefault(c => c.Name.Equals(ownerName, StringComparison.OrdinalIgnoreCase));

                    if (matchedChar != null)
                    {
                        var quantity = itemsSheet.LastColumnUsed().ColumnNumber() >= 5 ? row.Cell(5).GetValue<int>() : 1;
                        var isEquipped = itemsSheet.LastColumnUsed().ColumnNumber() >= 6 && row.Cell(6).GetValue<bool>();

                        matchedChar.Inventory.Add(new CharacterInventory
                        {
                            Id = Guid.NewGuid(),
                            CharacterId = matchedChar.Id,
                            ItemTemplateId = template.Id,
                            Item = template,
                            Quantity = quantity <= 0 ? 1 : quantity,
                            IsEquipped = isEquipped
                        });
                    }
                }
            }
        }

        // GUARDADO DE DATOS CON UNIT OF WORK
        // ==========================================
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. RAZAS
            var existingRaces = (await _unitOfWork.Races.GetAllAsync()).Select(r => r.Name.ToLower()).ToHashSet();
            var newRaces = racesList.Where(r => !existingRaces.Contains(r.Name.ToLower())).ToList();
            if (newRaces.Any()) await _unitOfWork.Races.AddRangeAsync(newRaces);

            // 2. CLASES
            var existingClasses = (await _unitOfWork.ClassDefinitions.GetAllAsync()).Select(c => c.Name.ToLower()).ToHashSet();
            var newClasses = classDefinitionList.Where(c => !existingClasses.Contains(c.Name.ToLower())).ToList();
            if (newClasses.Any()) await _unitOfWork.ClassDefinitions.AddRangeAsync(newClasses);

            // Guardado intermedio para asegurar las llaves de las clases
            await _unitOfWork.SaveChangesAsync();

            // 3. PROGRESIONES DE CLASES
            var dbProgressions = await _unitOfWork.ClassLevelProgressions.GetAllAsync();
            var existingProgKeys = dbProgressions.Select(p => $"{p.ClassDefId}_{p.Level}").ToHashSet();

            foreach (var prog in progressionsList)
            {
                var parentClass = classDefinitionList.FirstOrDefault(c => c.Id == prog.ClassDefId);
                if (parentClass == null) continue;

                var dbClass = await _unitOfWork.ClassDefinitions.GetByNameAsync(parentClass.Name);
                if (dbClass == null) continue;

                prog.ClassDefId = dbClass.Id;

                string key = $"{prog.ClassDefId}_{prog.Level}";
                if (!existingProgKeys.Contains(key))
                {
                    await _unitOfWork.ClassLevelProgressions.AddProgressionAsync(prog);
                    existingProgKeys.Add(key);
                }
                else
                {
                    // Recuperamos el ID real de la DB para que los personajes puedan jalar los rasgos
                    var match = dbProgressions.First(dp => $"{dp.ClassDefId}_{dp.Level}" == key);
                    prog.Id = match.Id;
                    prog.Features = match.Features; // Mantener la referencia a los rasgos de la DB
                }
            }

            // 4. REGLAS XP
            var existingXp = (await _unitOfWork.XpRules.GetAllAsync()).Select(x => x.Level).ToHashSet();
            var newXp = xpRulesList.Where(x => !existingXp.Contains(x.Level)).ToList();
            if (newXp.Any()) await _unitOfWork.XpRules.AddRangeAsync(newXp);

            // 5. DOTES (FEATS)
            var existingFeats = (await _unitOfWork.Feats.GetAllAsync()).Select(f => f.Name.ToLower()).ToHashSet();
            var newFeats = featsList.Where(f => !existingFeats.Contains(f.Name.ToLower())).ToList();
            if (newFeats.Any()) await _unitOfWork.Feats.AddRangeAsync(newFeats);

            // 6. HECHIZOS (SPELLS)
            var existingSpells = (await _unitOfWork.Spells.GetAllAsync()).Select(s => s.Name.ToLower()).ToHashSet();
            var newSpells = spellsList.Where(s => !existingSpells.Contains(s.Name.ToLower())).ToList();
            if (newSpells.Any()) await _unitOfWork.Spells.AddRangeAsync(newSpells);

            // 7. PLANTILLAS DE OBJETOS (ITEM TEMPLATES)
            var existingItems = (await _unitOfWork.ItemTemplates.GetAllAsync()).Select(i => i.Name.ToLower()).ToHashSet();
            var newItems = itemsList.Where(i => !existingItems.Contains(i.Name.ToLower())).ToList();
            if (newItems.Any()) await _unitOfWork.ItemTemplates.AddRangeAsync(newItems);

            // Guardado intermedio de catálogos
            await _unitOfWork.SaveChangesAsync();

            // 8. PROCESAR PERSONAJES 
            foreach (var character in charactersList)
            {
                var targetRaceName = racesList.FirstOrDefault(r => r.Id == character.RaceId)?.Name ?? string.Empty;
                var targetClassName = classDefinitionList.FirstOrDefault(c => c.Id == character.ClassDefId)?.Name ?? string.Empty;

                var dbRace = await _unitOfWork.Races.GetByNameAsync(targetRaceName);
                var dbClass = await _unitOfWork.ClassDefinitions.GetByNameAsync(targetClassName);

                character.RaceId = dbRace?.Id ?? character.RaceId;
                character.ClassDefId = dbClass?.Id ?? character.ClassDefId;

                // Limpieza preventiva para evitar duplicados en actualizaciones
                var existingChar = await _unitOfWork.Characters.GetByNameAsync(character.Name);
                if (existingChar != null)
                {
                    await _unitOfWork.Characters.RemoveAsync(existingChar);
                }

                if (dbClass != null && character.Level > 0)
                {
                    var earnedProgressions = progressionsList
                        .Where(p => p.ClassDefId == dbClass.Id && p.Level <= character.Level)
                        .ToList();

                    foreach (var prog in earnedProgressions)
                    {
                        if (prog.Features != null && prog.Features.Any())
                        {
                            foreach (var feature in prog.Features)
                            {
                                if (!character.AcquiredFeatures.Any(f => f.Id == feature.Id))
                                {
                                    character.AcquiredFeatures.Add(feature);
                                }
                            }
                        }
                    }
                }
                // Sincronizar también los items de la mochila
                foreach (var invItem in character.Inventory)
                {
                    var dbItem = await _unitOfWork.ItemTemplates.GetByNameAsync(invItem.Item.Name);
                    if (dbItem != null)
                    {
                        invItem.ItemTemplateId = dbItem.Id;
                        invItem.Item = dbItem;
                    }
                }
            }

            await _unitOfWork.Characters.AddRangeAsync(charactersList);


            // Guardado final definitivo libre de conflictos de llaves
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