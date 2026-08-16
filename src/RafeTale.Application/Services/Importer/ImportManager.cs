using ClosedXML.Excel;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Services.Importer.Initializer;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RafeTale.Application.Services.Importer;

public class ImportManager(IUnitOfWork unitOfWork, IDataExtractor dataExtractor, ILocalizationService localizationService, IAppInitializer appInitializer) : IExcelImportService
{
    public async Task<(int Count, string Version)> ImportDataFromExcelAsync(Stream excelStream)
    {
        var data = dataExtractor.ExtractAllAsync(excelStream);  
        

        // GUARDADO DE DATOS CON UNIT OF WORK
        // ==========================================
        await unitOfWork.BeginTransactionAsync();
        try
        {
            await SyncRacesAsync(data.Races);
            await SyncClassesAsync(data.ClassDefinitions);
            await unitOfWork.SaveChangesAsync();

            await SyncClassProgressionsAsync(data.ClassLevelProgressions, data.ClassDefinitions);

            await SyncSystemRulesAsync(data);
            await unitOfWork.SaveChangesAsync();

            await SyncSpellsAndItemsAsync(data.Spells, data.Items);
            await unitOfWork.SaveChangesAsync();

            await SyncCharactersAsync(data);
            await unitOfWork.SaveChangesAsync();

            await SyncLocalizationAndProficienciesAsync(data);
            await unitOfWork.SaveChangesAsync();



            appInitializer.UpdateStatus("¡Importación finalizada! Grimorio listo para la aventura...");
            await unitOfWork.CommitAsync();
            
            return (data.Characters.Count, Version: "1.0");
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
    private async Task SyncRacesAsync(List<Race> races)
    {
        if (races.Count == 0) return;

        var existing = (await unitOfWork.Races.GetAllAsync()).Select(r => r!.TechnicalName.ToLower()).ToHashSet();
        var newRaces = races.Where(r => !existing.Contains(r.TechnicalName.ToLower())).ToList();

        if (newRaces.Count > 0) await unitOfWork.Races.AddRangeAsync(newRaces);
        appInitializer.UpdateStatus("Razas importadas. Cargando clases...");
    }

    private async Task SyncClassesAsync(List<ClassDefinition> classes)
    {
        if (classes.Count == 0) return;

        var existing = (await unitOfWork.ClassDefinitions.GetAllAsync()).Select(c => c!.TechnicalName.ToLower()).ToHashSet();
        var newClasses = classes.Where(c => !existing.Contains(c.TechnicalName.ToLower())).ToList();

        if (newClasses.Count > 0) await unitOfWork.ClassDefinitions.AddRangeAsync(newClasses);
        appInitializer.UpdateStatus("Clases importadas. Sincronizando progresiones...");
    }

    private async Task SyncClassProgressionsAsync(List<ClassLevelProgression> progressions, List<ClassDefinition> rawClasses)
    {
        if (progressions.Count == 0) return;

        var dbProgressions = await unitOfWork.ClassLevelProgressions.GetAllAsync();
        var existingProgKeys = dbProgressions.Select(p => $"{p!.ClassDefId}_{p.Level}").ToHashSet();
        var dbClasses = await unitOfWork.ClassDefinitions.GetAllAsync();

        var updatedProgressionsList = new List<ClassLevelProgression>();

        foreach (var prog in progressions)
        {
            var parentClass = rawClasses.FirstOrDefault(c => c.Id == prog.ClassDefId);
            if (parentClass == null) continue;

            var dbClass = dbClasses.FirstOrDefault(x => x!.TechnicalName == parentClass.TechnicalName);
            if (dbClass == null) continue;

            prog.ClassDefId = dbClass.Id;
            string key = $"{prog.ClassDefId}_{prog.Level}";

            if (!existingProgKeys.Contains(key))
            {
                await unitOfWork.ClassLevelProgressions.AddAsync(prog);
                existingProgKeys.Add(key);
                updatedProgressionsList.Add(prog);
            }
            else
            {
                var match = dbProgressions.First(dp => $"{dp!.ClassDefId}_{dp.Level}" == key);
                updatedProgressionsList.Add(match!);
            }
        }
        progressions.Clear();
        progressions.AddRange(updatedProgressionsList);
        appInitializer.UpdateStatus("Progresiones de clase vinculadas. Sincronizando dotes y trasfondos...");
    }

    private async Task SyncSystemRulesAsync(dynamic data)
    {
        if (data.XpRules.Count > 0)
        {
            var existingXp = (await unitOfWork.XpRules.GetAllAsync()).Select(x => x!.Level).ToHashSet();
            var newXp = ((IEnumerable<XpRules>)data.XpRules).Where(x => !existingXp.Contains(x.Level)).ToList();
            if (newXp.Count > 0) await unitOfWork.XpRules.AddRangeAsync(newXp);
        }
        if (data.Feats.Count > 0)
        {
            var existingFeats = (await unitOfWork.Feats.GetAllAsync()).Select(f => f!.TechnicalName.ToLower()).ToHashSet();
            var newFeats = ((IEnumerable<Feat>)data.Feats).Where(f => !existingFeats.Contains(f.TechnicalName.ToLower())).ToList();
            if (newFeats.Count > 0) await unitOfWork.Feats.AddRangeAsync(newFeats);
        }
        if (data.Backgrounds.Count > 0)
        {
            var existingBackgrounds = (await unitOfWork.Backgrounds.GetAllAsync()).Select(i => i!.TechnicalName.ToLower()).ToHashSet();
            var newItems = ((IEnumerable<Background>)data.Backgrounds).Where(i => !existingBackgrounds.Contains(i.TechnicalName.ToLower())).ToList();
            if (newItems.Count > 0) await unitOfWork.Backgrounds.AddRangeAsync(newItems);
        }
        appInitializer.UpdateStatus("Reglas de experiencia, dotes y trasfondos importados. Guardando en base de datos...");
    }

    private async Task SyncSpellsAndItemsAsync(List<Spell> spells, List<ItemTemplate> items)
    {
        if (spells.Count > 0)
        {
            var existingSpells = (await unitOfWork.Spells.GetAllAsync()).Select(s => s!.TechnicalName.ToLower()).ToHashSet();
            var newSpells = spells.Where(s => !existingSpells.Contains(s.TechnicalName.ToLower())).ToList();
            if (newSpells.Count > 0) await unitOfWork.Spells.AddRangeAsync(newSpells);
        }
        if (items.Count > 0)
        {
            var existingItems = (await unitOfWork.ItemTemplates.GetAllAsync()).Select(i => i!.TechnicalName.ToLower()).ToHashSet();
            var newItems = items.Where(i => !existingItems.Contains(i.TechnicalName.ToLower())).ToList();
            if (newItems.Count > 0) await unitOfWork.ItemTemplates.AddRangeAsync(newItems);
        }
        appInitializer.UpdateStatus(" grimorios y almacenes sincronizados. Armando hojas de personajes...");
    }

    private async Task SyncCharactersAsync(dynamic data)
    {
        if (data.Characters.Count == 0) return;

        var raceDict = (await unitOfWork.Races.GetAllAsync()).ToDictionary(r => r!.TechnicalName.ToLower(), r => r);
        var classDict = (await unitOfWork.ClassDefinitions.GetAllAsync()).ToDictionary(c => c!.TechnicalName.ToLower(), c => c);
        var backgroundDict = (await unitOfWork.Backgrounds.GetAllAsync()).ToDictionary(b => b!.TechnicalName.ToLower().Trim(), b => b);

        foreach (Character character in data.Characters)
        {
            var targetRaceName = ((IEnumerable<Race>)data.Races).FirstOrDefault(r => r.Id == character.RaceId)?.TechnicalName ?? string.Empty;
            var targetClassName = ((IEnumerable<ClassDefinition>)data.ClassDefinitions).FirstOrDefault(c => c.Id == character.ClassDefId)?.TechnicalName ?? string.Empty;
            var targetBackgroundName = ((IEnumerable<Background>)data.Backgrounds).FirstOrDefault(b => b.Id == character.BackgroundId)?.TechnicalName ?? string.Empty;

            if (raceDict.TryGetValue(targetRaceName.ToLower(), out var dbRace)) character.RaceId = dbRace!.Id;
            if (classDict.TryGetValue(targetClassName.ToLower(), out var dbClass)) character.ClassDefId = dbClass!.Id;
            if (targetBackgroundName != string.Empty && backgroundDict.TryGetValue(targetBackgroundName.ToLower().Trim(), out var dbBackground)) character.BackgroundId = dbBackground!.Id;
            
            // Limpieza preventiva de personajes duplicados por nombre
            var existingChar = await unitOfWork.Characters.GetByNameAsync(character.Name);
            if (existingChar != null) await unitOfWork.Characters.RemoveAsync(existingChar);

            // Inyección de rasgos por nivel acumulado
            if (dbClass != null && character.Level > 0)
            {
                var earnedProgressions = ((IEnumerable<ClassLevelProgression>)data.ClassLevelProgressions)
                    .Where(p => p.ClassDefId == dbClass.Id && p.Level <= character.Level).ToList();

                foreach (var prog in earnedProgressions.Where(p => p.Features != null))
                {
                    foreach (var feature in prog.Features)
                    {
                        if (!character.AcquiredFeatures.Any(f => f.Id == feature.Id))
                            character.AcquiredFeatures.Add(feature);
                    }
                }
            }

            // Sincronizar herencia de ítems en la mochila del personaje
            foreach (var invItem in character.Inventory)
            {
                var dbItem = await unitOfWork.ItemTemplates.GetByNameAsync(invItem.Item.TechnicalName.Trim());
                if (dbItem == null) continue;
                invItem.ItemTemplateId = dbItem!.Id;
                invItem.Item = dbItem;
            }
        }

        await unitOfWork.Characters.AddRangeAsync(data.Characters);
        appInitializer.UpdateStatus("Personajes vinculados con éxito. Consolidando textos e idiomas...");
    }

    private async Task SyncLocalizationAndProficienciesAsync(dynamic data)
    {
        if (data.LocalizedContents.Count > 0) await unitOfWork.LocalizedContents.AddRangeAsync(data.LocalizedContents);

        if (data.SkillProficiencies.Count > 0)
        {
            var existingSkills = (await unitOfWork.Skills.GetAllAsync()).Select(x => x!.TechnicalName).ToHashSet();
            var newSkills = ((IEnumerable<Skill>)data.SkillProficiencies).Where(x => !existingSkills.Contains(x.TechnicalName)).ToList();
            if (newSkills.Count > 0) await unitOfWork.Skills.AddRangeAsync(newSkills);
        }
        if (data.Traits.Count > 0)
        {
            var existingTraits = (await unitOfWork.Traits.GetAllAsync()).Select(x => x!.TechnicalName).ToHashSet();
            var newTrait = ((IEnumerable<Trait>)data.Traits).Where(x => !existingTraits.Contains(x.TechnicalName)).ToList();
            if (newTrait.Count > 0) await unitOfWork.Traits.AddRangeAsync(newTrait);
        }
        if (data.Languages.Count > 0)
        {
            var existingLanguages = (await unitOfWork.Languages.GetAllAsync()).Select(x => x!.TechnicalName).ToHashSet();
            var newLanguages = ((IEnumerable<Language>)data.Languages).Where(x => !existingLanguages.Contains(x.TechnicalName)).ToList();
            if (newLanguages.Count > 0) await unitOfWork.Languages.AddRangeAsync(newLanguages);
        }
        if (data.SubRaces.Count > 0)
        {
            var existingSubRaces = (await unitOfWork.SubRaces.GetAllAsync()).Select(x => x!.TechnicalName).ToHashSet();
            var newSubRaces = ((IEnumerable<SubRace>)data.SubRaces).Where(x => !existingSubRaces.Contains(x.TechnicalName)).ToList();
            if (newSubRaces.Count > 0) await unitOfWork.SubRaces.AddRangeAsync(newSubRaces);
        }
        if (data.Subclasses.Count > 0)
        {
            var exisitnSubClases = (await unitOfWork.Subclasses.GetAllAsync()).Select(x => x!.TechnicalName).ToHashSet();
            var newSubClasses = ((IEnumerable<Subclass>)data.Subclasses).Where(x => !exisitnSubClases.Contains(x.TechnicalName)).ToList();
            if (newSubClasses.Count != 0) await unitOfWork.Subclasses.AddRangeAsync(newSubClasses);
        }

        // Sincronizar progresiones finales de las subclases (Arquetipos)
        if (data.SubclassLevelProgressions.Count > 0)
        {
            var dbSubProg = await unitOfWork.SubclassLevelProgressions.GetAllAsync();
            var existingSubProgKeys = dbSubProg.Select(p => $"{p!.SubclassId}_{p.Level}").ToHashSet();
            var dbSubclasses = await unitOfWork.Subclasses.GetAllAsync();

            foreach (var progression in (IEnumerable<SubclassLevelProgression>)data.SubclassLevelProgressions)
            {
                var parentSub = ((IEnumerable<Subclass>)data.Subclasses).FirstOrDefault(c => c.Id == progression.SubclassId);
                if (parentSub == null) continue;

                var dbSub = dbSubclasses.FirstOrDefault(x => x!.TechnicalName == parentSub.TechnicalName);
                if (dbSub == null) continue;

                string key = $"{progression.SubclassId}_{progression.Level}";
                if (!existingSubProgKeys.Contains(key))
                {
                    await unitOfWork.SubclassLevelProgressions.AddAsync(progression);
                    existingSubProgKeys.Add(key);
                }
                else
                {
                    var match = dbSubProg.First(dp => $"{dp!.SubclassId}_{dp.Level}" == key);
                    progression.Id = match!.Id;
                    progression.Features = match.Features;
                }
            }
        }
    }
}