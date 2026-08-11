using ClosedXML.Excel;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Modifiers;
using RafeTale.Infrastructure.Extraction.Extensions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.Json;
using System.Text.Json.Serialization;
using RafeTale.Infrastructure.Extraction.Sheets;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction
{
    public class ExcelDataExtractor : IDataExtractor
    {
        private IReadOnlyList<ISheetExtractor> _extractors = [];
        public ExcelDataExtractor(IEnumerable<ISheetExtractor> extractors)
        {
            _extractors = extractors.ToList();
        }
        private readonly LocLanguage _currentCulture;

        private Dictionary<string, LocalizedContent> _localizedContentCache = new();

        public ImportDataPackage ExtractAllAsync(Stream excelStream)
        {
            using var workbook = new XLWorkbook(excelStream);
            var context = new ExtractionContext(LocLanguage.es);
            
            foreach (var extractor in _extractors)
            {
                extractor.Extract(workbook, context);
            }

            context.Package.LocalizedContents.AddRange(context.Localization.GetAll());
            return context.Package;
        }
        public ImportDataPackage ExtractAllAsyncs(Stream excelStream)
        {
            var package = new ImportDataPackage();
            using var workbook = new XLWorkbook(excelStream);

            // Aquí llamas a tus lógicas pequeñas (lo que tenías en la función enorme)
            package.Languages = ExtractLanguages(workbook);
            package.Races = ExtractRaces(workbook, package.Languages);
            package.SubRaces = ExtractSubRaces(workbook, package.Races);
            package.Traits = ExtractTraits(workbook, package.Races, package.SubRaces);
            package.SpecialTraits = ExtractSpecialTraits(workbook, package.Traits);
            package.SkillProficiencies = ExtractSkillProficiencies(workbook);
            package.ClassDefinitions = ExtractClasses(workbook, package.SkillProficiencies);
            package.Subclasses = ExtractSubclasses(workbook, package.ClassDefinitions);
            package.Spells = ExtractSpells(workbook, package.ClassDefinitions.Select(x => x.TechnicalName).ToList());
            package.ClassLevelProgressions = ExtractClassLevelProgressions(workbook, package.ClassDefinitions);
            package.SubclassLevelProgressions = ExtractSubclassLevelProgressions(workbook, package.Subclasses);
            package.XpRules = ExtractXpRules(workbook);
            package.Feats = ExtractFeats(workbook);
            package.Backgrounds = ExtractBackgrounds(workbook, package.Feats);

            package.Characters = ExtractCharacters(workbook, package.Races, package.ClassDefinitions, package.Backgrounds);
            package.Items = ExtractItems(workbook, package.Characters);
            package.LocalizedContents.AddRange(_localizedContentCache.Values);
            return package;
        }

        public List<Skill> ExtractSkillProficiencies(IXLWorkbook workbook)
        {
            var skillProficiencyList = new List<Skill>();
            var sheet = workbook.GetSheetSafe("Skills");
            var rows = sheet?.RangeUsed()?.RowsUsed().Skip(1);
            if (rows == null)
                return skillProficiencyList;
            foreach (var row in rows)
            {
                var skill = new Skill
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString(),
                    Ability = ParseEnum<ASI>(row.Cell(3).GetString())
                };
                skillProficiencyList.Add(skill);
                SaveValidateLocalizedContent(skill.Id, LocEntity.Skill, LocProperty.Name, row.Cell(1).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(skill.Id, LocEntity.Skill, LocProperty.Ability, row.Cell(2).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(skill.Id, LocEntity.Skill, LocProperty.Description, row.Cell(3).GetString(), LocLanguage.en);

                SaveValidateLocalizedContent(skill.Id, LocEntity.Skill, LocProperty.Name, row.Cell(4).GetString(), _currentCulture);
                SaveValidateLocalizedContent(skill.Id, LocEntity.Skill, LocProperty.Ability, row.Cell(5).GetString(), _currentCulture);
                SaveValidateLocalizedContent(skill.Id, LocEntity.Skill, LocProperty.Description, row.Cell(6).GetString(), _currentCulture);
            }
            return skillProficiencyList;
        }

        public List<Language> ExtractLanguages(IXLWorkbook workbook)
        {
            var languagesList = new List<Language>();
            var sheet = workbook.GetSheetSafe("Languages");

            var rows = sheet?.RangeUsed()?.RowsUsed().Skip(1);
            if (rows == null)
                return languagesList;

            foreach (var row in rows)
            {
                var language = new Language
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString()
                };
                languagesList.Add(language);

                SaveValidateLocalizedContent(language.Id, LocEntity.Language, LocProperty.Name, row.Cell(1).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(language.Id, LocEntity.Language, LocProperty.Description, row.Cell(2).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(language.Id, LocEntity.Language, LocProperty.Name, row.Cell(3).GetString(), _currentCulture);
                SaveValidateLocalizedContent(language.Id, LocEntity.Language, LocProperty.Description, row.Cell(4).GetString(), _currentCulture);
            }

            return languagesList;
        }
        public List<Race> ExtractRaces(IXLWorkbook workbook, List<Language> allLanguages)
        {
            var raceList = new List<Race>();

            var raceSheet = workbook.GetSheetSafe("Races");
            var rows = raceSheet?.RangeUsed()?.RowsUsed().Skip(1);
            if (rows == null)
                return raceList;
            foreach (var row in rows)
            {
                var race = new Race
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString(),
                    CreatureType = ParseEnum<CreatureType>(row.Cell(2).GetString()),
                    Size = ParseEnum<SizeCategory>(row.Cell(3).GetString()),
                    Speed = row.Cell(4).GetValue<string>(),
                };

                SaveValidateLocalizedContent(race.Id, LocEntity.Race, LocProperty.Name, row.Cell(1).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(race.Id, LocEntity.Race, LocProperty.Description, row.Cell(7).GetString(), LocLanguage.en);

                SaveValidateLocalizedContent(race.Id, LocEntity.Race, LocProperty.Name, row.Cell(5).GetString(), _currentCulture);
                SaveValidateLocalizedContent(race.Id, LocEntity.Race, LocProperty.Description, row.Cell(6).GetString(), _currentCulture);

                var languagesInCell = row.Cell(8).GetString().Split(',').Select(l => l.Trim());

                foreach (var langName in languagesInCell)
                {
                    var foundLanguage = allLanguages.FirstOrDefault(l => l.TechnicalName.Equals(langName, StringComparison.OrdinalIgnoreCase));

                    if (foundLanguage != null) race.Languages.Add(foundLanguage);
                    if (foundLanguage == null) Console.WriteLine($"Advertencia: No se encontró el idioma '{langName}' para la raza");
                }
                raceList.Add(race);
            }
            return raceList;
        }
        public List<SubRace> ExtractSubRaces(IXLWorkbook workbook, List<Race> races)
        {
            var subRaces = new List<SubRace>();
            var sheet = workbook.GetSheetSafe("Sub Races");

            var rows = sheet.RangeUsed()!.RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var sub = new SubRace
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(2).GetString()
                };
                SaveValidateLocalizedContent(sub.Id, LocEntity.SubRace, LocProperty.Name, row.Cell(2).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(sub.Id, LocEntity.SubRace, LocProperty.Description, row.Cell(6).GetString(), LocLanguage.en);

                SaveValidateLocalizedContent(sub.Id, LocEntity.SubRace, LocProperty.Name, row.Cell(3).GetString(), _currentCulture);
                SaveValidateLocalizedContent(sub.Id, LocEntity.SubRace, LocProperty.Description, row.Cell(5).GetString(), _currentCulture);

                var race = races.FirstOrDefault(r => r.TechnicalName.Equals(row.Cell(1).GetString(), StringComparison.OrdinalIgnoreCase));

                if (race != null)
                    sub.RaceId = race.Id;

                subRaces.Add(sub);
            }
            return subRaces;
        }
        public List<Trait> ExtractTraits(IXLWorkbook workbook, List<Race> races, List<SubRace> subRace)
        {
            var sheet = workbook.GetSheetSafe("Traits");

            var traits = new List<Trait>();
            var rows = sheet.RangeUsed()!.RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var trait = new Trait
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString(),
                    RequiredLevel = row.Cell(2).TryGetValue<int>(out var res) ? res : 0,
                };
                if (row.Cell(3).GetString().Length > 0)
                    trait.Race = races.FirstOrDefault(r => r.TechnicalName.Equals(row.Cell(3).GetString(), StringComparison.OrdinalIgnoreCase))!;
                else
                    trait.Subrace = subRace.FirstOrDefault(r => r.TechnicalName.Equals(row.Cell(4).GetString(), StringComparison.OrdinalIgnoreCase))!;

                SaveValidateLocalizedContent(trait.Id, LocEntity.Trait, LocProperty.Name, trait.TechnicalName, LocLanguage.en);
                SaveValidateLocalizedContent(trait.Id, LocEntity.Trait, LocProperty.Description, row.Cell(7).GetString(), LocLanguage.en);

                SaveValidateLocalizedContent(trait.Id, LocEntity.Trait, LocProperty.Name, row.Cell(5).GetString(), _currentCulture);
                SaveValidateLocalizedContent(trait.Id, LocEntity.Trait, LocProperty.Description, row.Cell(6).GetString(), _currentCulture);

                traits.Add(trait);
            }
            return traits;
        }
        public List<SpecialTrait> ExtractSpecialTraits(IXLWorkbook workbook, List<Trait> traits)
        {
            var specialTraits = new List<SpecialTrait>();

            var sheet = workbook.GetSheetSafe("Special Traits");

            var rows = sheet.RangeUsed()?.RowsUsed().Skip(1);
            if (rows == null)
                return specialTraits;
            foreach (var row in rows)
            {
                var specialTrait = new SpecialTrait
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(2).GetString()
                };
                specialTrait.TraitId = traits.FirstOrDefault(r => r.TechnicalName.Equals(row.Cell(1).GetString(), StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty;

                SaveValidateLocalizedContent(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Name, specialTrait.TechnicalName, LocLanguage.en);
                SaveValidateLocalizedContent(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Description, row.Cell(3).GetString(), LocLanguage.en);

                SaveValidateLocalizedContent(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Name, row.Cell(5).GetString(), _currentCulture);
                SaveValidateLocalizedContent(specialTrait.Id, LocEntity.SpecialTrait, LocProperty.Description, row.Cell(6).GetString(), _currentCulture);

                specialTrait.Modifiers = GetModifierData(row.Cell(4).GetString());

                specialTraits.Add(specialTrait);
            }
            return specialTraits;
        }

        public List<ClassDefinition> ExtractClasses(IXLWorkbook workbook, List<Skill> skillProficiencies)
        {
            var classDefinitionList = new List<ClassDefinition>();
            var classSheet = workbook.GetSheetSafe("Classes");

            var rows = classSheet.RangeUsed()?.RowsUsed().Skip(1);
            if (rows == null)
                return classDefinitionList;
            foreach (var row in rows)
            {
                var classDef = new ClassDefinition
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString(),
                    HitDie = row.Cell(2).GetString(),
                    PrimaryAbility = ParseEnumList<ASI>(row.Cell(3).GetString()),
                    SavingThrowProficiencies = ParseEnumList<ASI>(row.Cell(4).GetString()),
                    ArmorProficiencies = ParseEnumList<ArmorProficiency>(row.Cell(5).GetString()),
                    WeaponProficiencies = ParseEnumList<WeaponProficiency>(row.Cell(6).GetString()),
                    ToolProficiencies = ParseEnumList<ToolProficiency>(row.Cell(7).GetString()),
                    SkillsToChoose = row.Cell(8).GetValue<int>(),
                };
                MapClassSkills(classDef, row.Cell(9).GetString(), skillProficiencies);

                classDefinitionList.Add(classDef);

                SaveValidateLocalizedContent(classDef.Id, LocEntity.Class, LocProperty.Name, classDef.TechnicalName, LocLanguage.en);
                SaveValidateLocalizedContent(classDef.Id, LocEntity.Class, LocProperty.Equipment, row.Cell(10).GetString(), _currentCulture);
                SaveValidateLocalizedContent(classDef.Id, LocEntity.Class, LocProperty.Name, row.Cell(11).GetString(), _currentCulture);
                SaveValidateLocalizedContent(classDef.Id, LocEntity.Class, LocProperty.Description, row.Cell(12).GetString(), _currentCulture);

            }
            return classDefinitionList;
        }

        public List<Character> ExtractCharacters(IXLWorkbook workbook, List<Race> races, List<ClassDefinition> classes, List<Background> backgrounds)
        {
            var charactersList = new List<Character>();
            var charSheet = workbook.GetSheetSafe("Personajes");
            var rows = charSheet?.RangeUsed()?.RowsUsed().Skip(1);
            if (rows == null)
                return charactersList;
            foreach (var row in rows)
            {
                var charName = row.Cell(1).GetString();
                var raceName = row.Cell(2).GetString();
                var className = row.Cell(3).GetString();

                var matchedRace = races.FirstOrDefault(r => r.TechnicalName.Equals(raceName, StringComparison.OrdinalIgnoreCase));
                var matchedClass = classes.FirstOrDefault(c => c.TechnicalName.Equals(className, StringComparison.OrdinalIgnoreCase));

                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = charName,
                    Level = row.Cell(4).GetValue<int>(),
                    Experience = row.Cell(5).GetValue<int>(),
                    RaceId = matchedRace?.Id ?? Guid.Empty,
                    ClassDefId = matchedClass?.Id ?? Guid.Empty,
                    AcquiredFeats = new List<Feat>(),
                    Stats = new Dictionary<string, int>(),
                    AcquiredFeatures = new List<Feature>(),
                    ActiveModifiers = new List<ActiveModifiers>(),
                    BackgroundId = backgrounds[0].Id,
                    Background = backgrounds[0],

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
            return charactersList;
        }

        public List<ClassLevelProgression> ExtractClassLevelProgressions(IXLWorkbook workbook, List<ClassDefinition> classes)
        {
            var progressionsList = new List<ClassLevelProgression>();
            var progressSheet = workbook.GetSheetSafe("ClassLevelProgression");
            var progressRows = progressSheet.RangeUsed()?.RowsUsed().Skip(1);
            if (progressRows == null)
                return progressionsList;
            foreach (var row in progressRows)
            {
                var className = row.Cell(1).GetString().Trim();
                var level = row.Cell(2).GetValue<int>();
                var proficiencyBonus = row.Cell(3).GetValue<int>();
                var featureTechnicalName = row.Cell(4).GetString().Trim();
                var progresionClassTraitDataRaw = row.Cell(5).GetString();
                var modifiersRaw = row.Cell(6).GetString();

                if (string.IsNullOrEmpty(featureTechnicalName)) continue;

                var targetClass = classes.FirstOrDefault(c => c.TechnicalName.Equals(className, StringComparison.OrdinalIgnoreCase));

                if (targetClass == null) continue;


                var feature = new Feature
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = featureTechnicalName,
                    Modifiers = GetModifierData(modifiersRaw),
                };

                SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Name, featureTechnicalName, LocLanguage.en);
                //SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Description, featureDescription, LocLanguage.en);

                SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Name, row.Cell(7).GetString(), _currentCulture);
                SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Description, row.Cell(8).GetString(), _currentCulture);
                var classTraits = GetClassTraits(progresionClassTraitDataRaw);
                if (!classTraits.Any())
                {

                }
                var existingProgression = progressionsList.FirstOrDefault(p => p.ClassDefId == targetClass.Id && p.Level == level);
                if (existingProgression != null)
                {
                    existingProgression.Features.Add(feature);
                    existingProgression.Traits.AddRange(classTraits);
                }
                else
                {
                    var newProgression = new ClassLevelProgression
                    {
                        Id = Guid.NewGuid(),
                        Level = level,
                        ClassDefId = targetClass.Id,
                        Features = new List<Feature> { feature },
                        Traits = classTraits
                    };

                    progressionsList.Add(newProgression);
                }
            }
            return progressionsList;
        }
        public List<Subclass> ExtractSubclasses(IXLWorkbook workbook, List<ClassDefinition> classDefinitions)
        {
            var subClassList = new List<Subclass>();
            var progressSheet = workbook.GetSheetSafe("SubClasses");
            var progressRows = progressSheet.RangeUsed()?.RowsUsed().Skip(1);

            foreach (var row in progressRows)
            {
                var classDefintionTechnicalName = row.Cell(1).GetString().Trim();
                var subClassTechnicalName = row.Cell(2).GetString().Trim();

                var classDefinition = classDefinitions.FirstOrDefault(p => p.TechnicalName == classDefintionTechnicalName);

                var subClass = new Subclass()
                {
                    Id = Guid.NewGuid(),
                    ClassDefinition = classDefinition!,
                    TechnicalName = subClassTechnicalName,
                    Progressions = []
                };

                if (classDefinition != null)
                {
                    classDefinition.Subclasses ??= [];
                    classDefinition.Subclasses.Add(subClass);
                }

                subClassList.Add(subClass);
                SaveValidateLocalizedContent(subClass.Id, LocEntity.Subclass, LocProperty.Name, subClassTechnicalName, LocLanguage.en);
                SaveValidateLocalizedContent(subClass.Id, LocEntity.Subclass, LocProperty.Description, row.Cell(3).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(subClass.Id, LocEntity.Subclass, LocProperty.Name, row.Cell(4).GetString(), _currentCulture);
                SaveValidateLocalizedContent(subClass.Id, LocEntity.Subclass, LocProperty.Description, row.Cell(5).GetString(), _currentCulture);

            }
            return subClassList;
        }

        public List<SubclassLevelProgression> ExtractSubclassLevelProgressions(IXLWorkbook workbook, List<Subclass> subclasses)
        {
            var progressionsList = new List<SubclassLevelProgression>();
            var progressSheet = workbook.GetSheetSafe("SubClassLevelProgresion");
            var progressRows = progressSheet.RangeUsed()?.RowsUsed().Skip(1);

            foreach (var row in progressRows)
            {
                var classDefName = row.Cell(1).GetString().Trim();
                var subclassName = row.Cell(2).GetString().Trim();
                var featureTechnicalName = row.Cell(3).GetString().Trim();
                var level = row.Cell(4).GetValue<int>();
                var modifiersRaw = row.Cell(5).GetString();

                var targetSubclass = subclasses.FirstOrDefault(c => c.TechnicalName.Equals(subclassName, StringComparison.OrdinalIgnoreCase));

                if (targetSubclass == null)
                    continue;

                var feature = new Feature
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = featureTechnicalName,
                    RequiresChoice = featureTechnicalName.Contains("Elegir", StringComparison.OrdinalIgnoreCase) ||
                         featureTechnicalName.Contains("Arquetipo", StringComparison.OrdinalIgnoreCase),
                    //Modifiers = GetModifierData(modifiersRaw)
                };

                SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Name, featureTechnicalName, LocLanguage.en);
                //SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Description, featureDescription, LocLanguage.en);

                SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Name, row.Cell(6).GetString(), _currentCulture);
                SaveValidateLocalizedContent(feature.Id, LocEntity.Feature, LocProperty.Description, row.Cell(7).GetString(), _currentCulture);


                var existingProgression = progressionsList.FirstOrDefault(p => p.SubclassId == targetSubclass.Id && p.Level == level);
                if (existingProgression != null)
                {
                    existingProgression.Features.Add(feature);
                }
                else
                {
                    var newProgression = new SubclassLevelProgression
                    {
                        Id = Guid.NewGuid(),
                        Level = level,
                        SubclassId = targetSubclass.Id,
                        Subclass = targetSubclass,
                        Features = new List<Feature> { feature } // <-- Metemos el Feature real con sus datos

                    };
                    targetSubclass.Progressions ??= new List<SubclassLevelProgression>();
                    targetSubclass.Progressions.Add(newProgression);

                    progressionsList.Add(newProgression);
                }
            }
            return progressionsList;
        }
        public List<Spell> ExtractSpells(IXLWorkbook workbook, List<string> classDefinitions)
        {
            var spellsList = new List<Spell>();
            var spellSheet = workbook.GetSheetSafe("Spells");

            var rows = spellSheet.RangeUsed()?.RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var spell = new Spell
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString() ?? string.Empty,
                    Level = ParseEnum<SpellLevel>(row.Cell(2).GetString()),
                    School = ParseEnum<SchoolOfMagicEnum>(row.Cell(3).GetString()),
                    CastingTime = ParseEnum<CastingTime>(row.Cell(4).GetString()),
                    Range = ParseEnum<SpellRange>(row.Cell(5).GetString()),
                    RangeDistance = row.Cell(6).GetValue<string>(),
                    Components = ParseEnumList<SpellComponent>(row.Cell(7).GetString()),
                    Duration = ParseEnumList<SpellDuration>(row.Cell(9).GetString()),
                    Concentration = ParseEnum<SpellConcentration>(row.Cell(10).GetString()),
                    Ritual = row.Cell(11).GetString().Equals("Si", StringComparison.OrdinalIgnoreCase),

                };
                if (spell.Level == SpellLevel.Cantrip)
                {

                }
                MapSpellClass(spell, row.Cell(12).GetString(), classDefinitions);

                SaveValidateLocalizedContent(spell.Id, LocEntity.Spell, LocProperty.Name, spell.TechnicalName, LocLanguage.en);
                SaveValidateLocalizedContent(spell.Id, LocEntity.Spell, LocProperty.Description, row.Cell(12).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(spell.Id, LocEntity.Spell, LocProperty.MaterialComponentDescription, row.Cell(8).GetString(), LocLanguage.en);///todo 

                SaveValidateLocalizedContent(spell.Id, LocEntity.Spell, LocProperty.Name, row.Cell(13).GetString(), _currentCulture);
                SaveValidateLocalizedContent(spell.Id, LocEntity.Spell, LocProperty.Description, row.Cell(14).GetString(), _currentCulture);
                SaveValidateLocalizedContent(spell.Id, LocEntity.Spell, LocProperty.MaterialComponentDescription, row.Cell(15).GetString(), _currentCulture);


                spellsList.Add(spell);
            }
            return spellsList;
        }

        public List<XpRules> ExtractXpRules(IXLWorkbook workbook)
        {
            var xpRulesList = new List<XpRules>();
            var xpSheet = workbook.GetSheetSafe("ReglasXP");

            var rows = xpSheet.RangeUsed()?.RowsUsed().Skip(1);
            if (rows == null)
                return xpRulesList;
            foreach (var row in rows)
            {
                xpRulesList.Add(new XpRules
                {
                    Level = row.Cell(1).GetValue<int>(),
                    RequiredXp = row.Cell(2).GetValue<int>(),
                    Bonus = xpSheet.LastColumnUsed().ColumnNumber() >= 3 ? row.Cell(3).GetValue<int>() : 0
                });
            }
            return xpRulesList;
        }

        public List<Feat> ExtractFeats(IXLWorkbook workbook)
        {
            var featsList = new List<Feat>();
            var featSheet = workbook.GetSheetSafe("Feats");

            var rows = featSheet.RangeUsed()?.RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var featPrerequisite = row.Cell(2).GetString() == "none" ? null : row.Cell(2).GetString();
                var featModifiersRaw = row.Cell(3).GetString() == "none" ? null : row.Cell(3).GetString();

                var feat = new Feat
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString() ?? string.Empty,
                    Prerequisite = GetPrerequisiteModifierData(featPrerequisite),
                    Modifiers = GetModifierData(featModifiersRaw),
                    Category = ParseEnum<CategoryFeat>(row.Cell(4).GetString())
                };
                featsList.Add(feat);

                SaveValidateLocalizedContent(feat.Id, LocEntity.Feat, LocProperty.Name, feat.TechnicalName, LocLanguage.en);
                SaveValidateLocalizedContent(feat.Id, LocEntity.Feat, LocProperty.Description, row.Cell(5).GetString(), LocLanguage.en);

                SaveValidateLocalizedContent(feat.Id, LocEntity.Feat, LocProperty.Name, row.Cell(6).GetString(), _currentCulture);
                SaveValidateLocalizedContent(feat.Id, LocEntity.Feat, LocProperty.Description, row.Cell(7).GetString(), _currentCulture);

            }
            return featsList;
        }

        public List<ItemTemplate> ExtractItems(IXLWorkbook workbook, List<Character> characters)
        {
            var itemsList = new List<ItemTemplate>();
            var itemsSheet = workbook.GetSheetSafe("Items");

            var rows = itemsSheet.RangeUsed()?.RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var itemName = row.Cell(1).GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(itemName)) continue;

                var template = new ItemTemplate
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = itemName,
                    Category = itemsSheet.LastColumnUsed().ColumnNumber() >= 3
                        ? (ItemCategory)Enum.Parse(typeof(ItemCategory), row.Cell(3).GetString(), true)
                        : ItemCategory.AdventuringGear
                };
                SaveValidateLocalizedContent(template.Id, LocEntity.ItemTemplate, LocProperty.Name, itemName, _currentCulture);
                SaveValidateLocalizedContent(template.Id, LocEntity.ItemTemplate, LocProperty.Description, row.Cell(2).GetString(), _currentCulture);



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
            return itemsList;
        }
        public List<SchoolOfMagic> ExtractSchoolsOfMagic(IXLWorkbook workbook)
        {
            var schoolsList = new List<SchoolOfMagic>();
            var sheet = workbook.GetSheetSafe("SchoolsOfMagic");
            var rows = sheet.RangeUsed()?.RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var school = new SchoolOfMagic
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = ParseEnum<SchoolOfMagicEnum>(row.Cell(1).GetString())
                };
                schoolsList.Add(school);
                SaveValidateLocalizedContent(school.Id, LocEntity.SchoolOfMagic, LocProperty.Name, row.Cell(1).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(school.Id, LocEntity.SchoolOfMagic, LocProperty.Description, row.Cell(2).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(school.Id, LocEntity.SchoolOfMagic, LocProperty.Name, row.Cell(3).GetString(), _currentCulture);
                SaveValidateLocalizedContent(school.Id, LocEntity.SchoolOfMagic, LocProperty.Description, row.Cell(4).GetString(), _currentCulture);
            }
            return schoolsList;
        }
        public List<Background> ExtractBackgrounds(IXLWorkbook workbook, List<Feat> feats)
        {
            var backgroundsList = new List<Background>();
            var sheet = workbook.GetSheetSafe("Backgrounds");
            var rows = sheet.RangeUsed()?.RowsUsed().Skip(1);
            foreach (var row in rows)
            {
                var background = new Background
                {
                    Id = Guid.NewGuid(),
                    TechnicalName = row.Cell(1).GetString(),
                    ASIs = ParseEnumList<ASI>(row.Cell(2).GetString()),
                    SkillProficiencies = ParseEnumList<SkillType>(row.Cell(4).GetString())
                };

                var featName = row.Cell(3).GetString();
                if (!string.IsNullOrEmpty(featName))
                {
                    var feat = feats.FirstOrDefault(f => f.TechnicalName.Equals(featName, StringComparison.OrdinalIgnoreCase));
                    if (feat != null)
                    {
                        background.FeatId = feat.Id;
                        background.Feat = feat;
                    }
                    else
                    {
                        Console.WriteLine($"Advertencia: No se encontró el rasgo '{featName}' para el trasfondo '{background.TechnicalName}'");
                    }
                }

                backgroundsList.Add(background);

                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.Name, background.TechnicalName, LocLanguage.en);
                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.ToolProficiencies, row.Cell(5).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.Equipment, row.Cell(6).GetString(), LocLanguage.en);
                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.Description, row.Cell(7).GetString(), LocLanguage.en);

                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.Name, row.Cell(8).GetString(), _currentCulture);
                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.ToolProficiencies, row.Cell(9).GetString(), _currentCulture);
                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.Equipment, row.Cell(10).GetString(), _currentCulture);
                SaveValidateLocalizedContent(background.Id, LocEntity.Background, LocProperty.Description, row.Cell(11).GetString(), _currentCulture);
            }
            return backgroundsList;
        }


        private void SaveValidateLocalizedContent(Guid entityId, LocEntity entityType, LocProperty property, string text, LocLanguage languageCode)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            string key = $"{entityId}_{property}_{languageCode}";

            if (_localizedContentCache.TryGetValue(key, out var existing))
            {
                existing.Text = text;
            }
            else
            {
                var extractedContent = ExtractLocalizedContent(entityId, entityType, property, text, languageCode);
                _localizedContentCache.Add(key, extractedContent);
            }
        }
        private LocalizedContent ExtractLocalizedContent(Guid entityId, LocEntity entityType, LocProperty property, string text, LocLanguage LanguageCode)
        {
            return new LocalizedContent { Id = Guid.NewGuid(), EntityId = entityId, EntityType = entityType, Property = property, Text = text, LanguageCode = LanguageCode };
        }
        private List<T> ParseEnumList<T>(string input) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(input)) return new List<T>();

            return input.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => Enum.TryParse<T>(s, true, out _))
                        .Select(s => Enum.Parse<T>(s, true))
                        .Cast<T>()
                        .ToList();
        }
        private T ParseEnum<T>(string input) where T : struct, Enum
        {
            return Enum.TryParse<T>(input.Trim(), true, out var result) ? result : default;
        }
        private void MapClassSkills(ClassDefinition classDef, string rawSkills, List<Skill> allSkills)
        {
            var skillNames = rawSkills.Split(',').Select(s => s.Trim());
            if (skillNames.Contains("Any"))
            {
                classDef.SkillProficiencies.AddRange(allSkills);
                return;
            }
            foreach (var name in skillNames)
            {
                var matched = allSkills.FirstOrDefault(s => s.TechnicalName.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (matched != null) classDef.SkillProficiencies.Add(matched);
            }
        }
        private void MapSpellClass(Spell spell, string rawClass, List<string> allClasses)
        {
            var classNames = rawClass.Split(',').Select(s => s.Trim());
            if (classNames.Contains("Any"))
            {
                spell.ClassesTechnicalNames.AddRange(allClasses);
                return;
            }
            foreach (var name in classNames)
            {
                var matched = allClasses.FirstOrDefault(s => s.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (matched != null) spell.ClassesTechnicalNames.Add(matched);
            }
        }
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private List<ModifierData> GetModifierData(string modifiers)
        {
            try
            {
                return string.IsNullOrWhiteSpace(modifiers)
                            ? new List<ModifierData>()
                            : JsonSerializer.Deserialize<List<ModifierData>>(modifiers, _jsonOptions)
                              ?? new List<ModifierData>();
            }
            catch (JsonException)
            {
                Console.WriteLine($"Error de JSON: {modifiers}. Revisa el formato en el Excel.");
                return new List<ModifierData>();
            }
        }

        private List<FeatPrerequisiteModifierData> GetPrerequisiteModifierData(string featPrerequisite)
        {
            try
            {
                return string.IsNullOrWhiteSpace(featPrerequisite)
                            ? new List<FeatPrerequisiteModifierData>()
                            : JsonSerializer.Deserialize<List<FeatPrerequisiteModifierData>>(featPrerequisite, _jsonOptions)
                              ?? new List<FeatPrerequisiteModifierData>();
            }
            catch (JsonException)
            {
                Console.WriteLine($"Error de JSON: {featPrerequisite}. Revisa el formato en el Excel.");
                return new List<FeatPrerequisiteModifierData>();
            }
        }

        private List<ClassTrait> GetClassTraits(string classTraitDataRaw)
        {
            List<ClassTrait> traits = new List<ClassTrait>();

            var pairs = classTraitDataRaw.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(':', 2);
                if (keyValue.Length != 2) continue;

                string keyStr = keyValue[0].Trim();
                string valueStr = keyValue[1].Trim();

                var trait = new ClassTrait();
                trait.Type = ParseEnum<ResourceType>(keyStr);
                // 1. Tratamiento especial para la matriz de hechizos
                if (keyStr.Equals("SpellSlots", StringComparison.OrdinalIgnoreCase))
                {
                    var slots = JsonSerializer.Deserialize<int[]>(valueStr);
                    trait.SpellSlots = slots ?? new int[9];
                    trait.Value = null; // Opcional, asegurar limpieza
                }
                else
                {
                    trait.Value = valueStr;
                    trait.SpellSlots = null;
                }

                traits.Add(trait);
            }
            return traits;
        }
    }
}
