using ClosedXML.Excel;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Services;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Modifiers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LanguageType = DnDreams.Domain.Enums.LanguageType;

namespace DnDreams.Infrastructure.Extractors
{
    public class ExcelDataExtractor : IDataExtractor
    {
        public async Task<ImportDataPackage> ExtractAllAsync(Stream excelStream)
        {
            var package = new ImportDataPackage();
            using var workbook = new XLWorkbook(excelStream);

            // Aquí llamas a tus lógicas pequeñas (lo que tenías en la función enorme)
            package.Races = ExtractRaces(workbook);
            package.SubRaces = ExtractSubRaces(workbook, package.Races);
            package.ClassDefinitions = ExtractClasses(workbook);
            package.Spells = ExtractSpells(workbook, package.ClassDefinitions);
            package.ClassLevelProgressions = ExtractClassLevelProgressions(workbook, package.ClassDefinitions);
            package.XpRules = ExtractXpRules(workbook);
            package.Feats = ExtractFeats(workbook);
            package.Characters = ExtractCharacters(workbook, package.Races, package.ClassDefinitions);
            package.Items = ExtractItems(workbook, package.Characters);

            return package;
        }
        public List<Race> ExtractRaces(IXLWorkbook workbook)
        {
            var raceList = new List<Race>();

            if (workbook.TryGetWorksheet("Razas", out var raceSheet))
            {
                var rows = raceSheet.RangeUsed().RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    var race = new Race
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(1).GetString(),
                        Speed = row.Cell(2).GetValue<float>(),
                        Size = Enum.TryParse<SizeCategory>(row.Cell(3).GetString(), true, out var size) ? size : SizeCategory.Medium,
                        Description = row.Cell(4).GetString(),
                        CreatureType = Enum.TryParse<CreatureType>(row.Cell(5).GetString(), true, out var type) ? type : CreatureType.Humanoid,
                        Darkvision = row.Cell(6).GetString(),
                        Resistances = row.Cell(7).GetString(),
                        RacialTraits = row.Cell(9).GetString()
                    };

                    var languagesList = row.Cell(8).GetString().Split(',').Select(l => l.Trim()).ToList();
                    foreach (var lang in languagesList)
                    {
                        if (Enum.TryParse<LanguageType>(lang, true, out var parsedLang))
                        {
                            race.Languages.Add(parsedLang);
                        }
                    }

                    for (int col = 10; col <= raceSheet.LastColumnUsed().ColumnNumber(); col++)
                    {
                        var statName = raceSheet.Cell(1, col).GetString();
                        var statValue = row.Cell(col).GetValue<int>();
                        if (!string.IsNullOrEmpty(statName) && statValue != 0)
                        {
                            race.StatBonuses[statName] = statValue;
                        }
                    }
                    raceList.Add(race);
                }
            }
            return raceList;
        }

        public List<ClassDefinition> ExtractClasses(IXLWorkbook workbook)
        {
            var classDefinitionList = new List<ClassDefinition>();
            if (workbook.TryGetWorksheet("Clases", out var classSheet))
            {
                var rows = classSheet.RangeUsed().RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    classDefinitionList.Add(new ClassDefinition
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(1).GetString(),
                        HitDie = row.Cell(2).GetString(),
                        PrimaryAbility = row.Cell(3).GetString(),
                        SavingThrowProficiencies = row.Cell(4).GetString(),
                        ArmorProficiencies = row.Cell(5).GetString(),
                        WeaponProficiencies = row.Cell(6).GetString(),
                        SkillsToChoose = row.Cell(7).GetValue<int>(),
                        SkillProficiencies = row.Cell(8).GetString()
                    });
                }
            }
            return classDefinitionList;
        }

        public List<Character> ExtractCharacters(IXLWorkbook workbook, List<Race> races, List<ClassDefinition> classes)
        {
            var charactersList = new List<Character>();
            if (workbook.TryGetWorksheet("Personajes", out var charSheet))
            {
                var rows = charSheet.RangeUsed().RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    var charName = row.Cell(1).GetString();
                    var raceName = row.Cell(2).GetString();
                    var className = row.Cell(3).GetString();

                    var matchedRace = races.FirstOrDefault(r => r.Name.Equals(raceName, StringComparison.OrdinalIgnoreCase));
                    var matchedClass = classes.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

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
            return charactersList;
        }

        public List<ClassLevelProgression> ExtractClassLevelProgressions(IXLWorkbook workbook, List<ClassDefinition> classes)
        {
            var progressionsList = new List<ClassLevelProgression>();
            if (workbook.TryGetWorksheet("ProgresoClases", out var progressSheet))
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() } // <-- LA CLAVE
                };

                var progressRows = progressSheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in progressRows)
                {
                    var className = row.Cell(1).GetString().Trim();
                    var level = row.Cell(2).GetValue<int>();
                    var featureName = row.Cell(3).GetString().Trim();
                    var featureDescription = row.Cell(4).GetString().Trim() ?? $"Rasgo de nivel {level}";
                    var modifiersRaw = row.Cell(5).GetString();
                    var specialData = row.Cell(6).GetString();

                    if (string.IsNullOrEmpty(featureName)) continue;

                    var targetClass = classes.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

                    if (targetClass == null) continue;

                    var feature = new Feature
                    {
                        Id = Guid.NewGuid(),
                        Name = featureName,
                        Description = featureDescription,
                        RequiresChoice = featureName.Contains("Elegir", StringComparison.OrdinalIgnoreCase) ||
                             featureName.Contains("Arquetipo", StringComparison.OrdinalIgnoreCase),
                        Modifiers = string.IsNullOrWhiteSpace(modifiersRaw)
                            ? new List<ModifierData>()
                            : JsonSerializer.Deserialize<List<ModifierData>>(modifiersRaw, jsonOptions)
                              ?? new List<ModifierData>(),
                        SpecialData = specialData ?? string.Empty,
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
            return progressionsList;
        }

        public List<Spell> ExtractSpells(IXLWorkbook workbook, List<ClassDefinition> classDefinitions)
        {
            var spellsList = new List<Spell>();

            if (workbook.TryGetWorksheet("Hechizos", out var spellSheet))
            {
                var rows = spellSheet.RangeUsed().RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    var spell = new Spell
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(1).GetString() ?? string.Empty,
                        Level = row.Cell(2).GetValue<int>(),
                        School = row.Cell(3).GetString() ?? string.Empty,
                        CastingTime = row.Cell(4).GetString() ?? string.Empty,
                        Range = row.Cell(5).GetString() ?? string.Empty,
                        Description = row.Cell(6).GetString() ?? string.Empty,
                        Components = row.Cell(7).GetString() ?? string.Empty,
                        Duration = row.Cell(8).GetValue<string>(),
                        Concentration = row.Cell(9).GetString() ?? string.Empty,
                        Ritual = row.Cell(10).GetString().Equals("Si", StringComparison.OrdinalIgnoreCase),
                    };

                    var clasesList = row.Cell(11).GetString().Split(',').Select(l => l.Trim()).ToList();
                    foreach (var className in clasesList)
                    {
                        var matchedClass = classDefinitions.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
                        if (matchedClass != null)
                        {
                            spell.Classes.Add(matchedClass);
                        }
                    }

                    spellsList.Add(spell);
                }
            }
            return spellsList;
        }

        public List<XpRules> ExtractXpRules(IXLWorkbook workbook)
        {
            var xpRulesList = new List<XpRules>();
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
            return xpRulesList;
        }

        public List<Feat> ExtractFeats(IXLWorkbook workbook)
        {
            var featsList = new List<Feat>();
            if (workbook.TryGetWorksheet("Dotes", out var featSheet))
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() } // <-- LA CLAVE
                };

                var rows = featSheet.RangeUsed().RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    var featModifiersRaw = row.Cell(4).GetString();

                    featsList.Add(new Feat
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(1).GetString() ?? string.Empty,
                        Description = row.Cell(2).GetString() ?? string.Empty,
                        Prerequisite = row.Cell(3).GetString() ?? "Ninguno",
                        Modifiers = string.IsNullOrWhiteSpace(featModifiersRaw)
                            ? new List<ModifierData>()
                            : JsonSerializer.Deserialize<List<ModifierData>>(featModifiersRaw, jsonOptions)
                              ?? new List<ModifierData>()
                    });
                }
            }
            return featsList;
        }

        public List<ItemTemplate> ExtractItems(IXLWorkbook workbook, List<Character> characters)
        {
            var itemsList = new List<ItemTemplate>();
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
                        var matchedChar = characters.FirstOrDefault(c => c.Name.Equals(ownerName, StringComparison.OrdinalIgnoreCase));

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
            return itemsList;
        }

        public List<SubRace> ExtractSubRaces(IXLWorkbook workbook, List<Race> races)
        {
            var subRaces = new List<SubRace>();
            if (workbook.TryGetWorksheet("Sub Razas", out var itemsSheet))
            {
                var rows = itemsSheet.RangeUsed().RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    var sub = new SubRace
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(2).GetString() ?? string.Empty,
                        Description = row.Cell(3).GetString() ?? string.Empty,
                    };
                    sub.RaceId = races.FirstOrDefault(r => r.Name.Equals(row.Cell(1).GetString(), StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty;

                    subRaces.Add(sub);
                }
            }
            return subRaces;
        }
    }
}
