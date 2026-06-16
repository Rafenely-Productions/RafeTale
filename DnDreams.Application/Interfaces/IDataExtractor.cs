using ClosedXML.Excel;
using DnDreams.Application.Services.Importer;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;

namespace DnDreams.Application.Interfaces
{
    public interface IDataExtractor
    {
        Task<ImportDataPackage> ExtractAllAsync(Stream excelStream);

        List<Language> ExtractLanguages(IXLWorkbook workbook);
        List<Race> ExtractRaces(IXLWorkbook workbook, List<Language> languages);
        List<SubRace> ExtractSubRaces(IXLWorkbook workbook, List<Race> races);
        List<ClassDefinition> ExtractClasses(IXLWorkbook workbook,List<Skill> skillProficiencies);
        List<Skill> ExtractSkillProficiencies(IXLWorkbook workbook);

        List<Character> ExtractCharacters(IXLWorkbook workbook, List<Race> races, List<ClassDefinition> classes);
        List<ClassLevelProgression> ExtractClassLevelProgressions(IXLWorkbook workbook, List<ClassDefinition> classes);
        List<Spell> ExtractSpells(IXLWorkbook workbook,List<ClassDefinition> classDefinitions);
        List<XpRules> ExtractXpRules(IXLWorkbook workbook);
        List<Feat> ExtractFeats(IXLWorkbook workbook);
        List<ItemTemplate> ExtractItems(IXLWorkbook workbook, List<Character> characters);
        List<Trait> ExtractTraits(IXLWorkbook workbook, List<Race> races);
        List<SpecialTrait> ExtractSpecialTraits(IXLWorkbook workbook, List<Trait> traits);
        List<SchoolOfMagic> ExtractSchoolsOfMagic(IXLWorkbook workbook);
        List<Background> ExtractBackgrounds(IXLWorkbook workbook, List<Feat> feats);
        List<Subclass> ExtractSubclasses(IXLWorkbook workbook, List<ClassDefinition> classDefinitions);
        List<SubclassLevelProgression> ExtractSubclassLevelProgressions(IXLWorkbook workbook, List<Subclass> subclasses);
    }
}
