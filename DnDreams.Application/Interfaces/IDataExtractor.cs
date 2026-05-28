using ClosedXML.Excel;
using DnDreams.Application.Services;
using DnDreams.Domain.Entities;

namespace DnDreams.Application.Interfaces
{
    public interface IDataExtractor
    {
        Task<ImportDataPackage> ExtractAllAsync(Stream excelStream);

        List<Race> ExtractRaces(IXLWorkbook workbook);
        List<SubRace> ExtractSubRaces(IXLWorkbook workbook, List<Race> races);
        List<ClassDefinition> ExtractClasses(IXLWorkbook workbook);

        List<Character> ExtractCharacters(IXLWorkbook workbook, List<Race> races, List<ClassDefinition> classes);
        List<ClassLevelProgression> ExtractClassLevelProgressions(IXLWorkbook workbook, List<ClassDefinition> classes);
         List<Spell> ExtractSpells(IXLWorkbook workbook,List<ClassDefinition> classDefinitions);
        List<XpRules> ExtractXpRules(IXLWorkbook workbook);
        List<Feat> ExtractFeats(IXLWorkbook workbook);
        List<ItemTemplate> ExtractItems(IXLWorkbook workbook, List<Character> characters);
    }
}
