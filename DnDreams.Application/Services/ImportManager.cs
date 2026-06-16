using ClosedXML.Excel;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Services.Initializer;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using DnDreams.Domain.Modifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DnDreams.Application.Services;

public class ImportManager : IExcelImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private IDataExtractor _dataExtractor;
    private ILocalizationService _localizationService;

    public ImportManager(IUnitOfWork unitOfWork, IDataExtractor dataExtractor, ILocalizationService localizationService)
    {
        _unitOfWork = unitOfWork;
        _dataExtractor = dataExtractor;
        _localizationService = localizationService;
    }

    public async Task<(int Count, string Version)> ImportDataFromExcelAsync(Stream excelStream,AppInitializer appInitializer)
    {
        var data = await _dataExtractor.ExtractAllAsync(excelStream);  
        
        // GUARDADO DE DATOS CON UNIT OF WORK
        // ==========================================
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (data.Races.Any())
            {
                var existingRaces = (await _unitOfWork.Races.GetAllAsync()).Select(r=> r.TechnicalName.ToLower()).ToHashSet();
                var newRaces = data.Races.Where(r => !existingRaces.Contains(r.TechnicalName.ToLower())).ToList();
                if (newRaces.Any()) await _unitOfWork.Races.AddRangeAsync(newRaces);
            }
            appInitializer.UpdateStatus("Razas importadas. Cargando clases...");
            if (data.ClassDefinitions.Any())
            {
                var existingClasses = (await _unitOfWork.ClassDefinitions.GetAllAsync()).Select(c => c.TechnicalName.ToLower()).ToHashSet();
                var newClasses = data.ClassDefinitions.Where(c => !existingClasses.Contains(c.TechnicalName.ToLower())).ToList();
                if (newClasses.Any()) await _unitOfWork.ClassDefinitions.AddRangeAsync(newClasses);
            }
            appInitializer.UpdateStatus("Clases importadas. Guardando Razas y Clases...");
            await _unitOfWork.SaveChangesAsync();
            appInitializer.UpdateStatus("Razas y clases guardas en la base de datos. Sincronizando progresiones...");
            if (data.ClassLevelProgressions.Any())
            {
                var dbProgressions = await _unitOfWork.ClassLevelProgressions.GetAllAsync();
                var existingProgKeys = dbProgressions.Select(p => $"{p.ClassDefId}_{p.Level}").ToHashSet();

                foreach (var prog in data.ClassLevelProgressions)
                {
                    var parentClass = data.ClassDefinitions.FirstOrDefault(c => c.Id == prog.ClassDefId);
                    if (parentClass == null) continue;

                    var dbClass = await _unitOfWork.ClassDefinitions.GetSingleAsync(x=> x.TechnicalName == parentClass.TechnicalName);
                    if (dbClass == null) continue;

                    prog.ClassDefId = dbClass.Id;

                    string key = $"{prog.ClassDefId}_{prog.Level}";
                    if (!existingProgKeys.Contains(key))
                    {
                        await _unitOfWork.ClassLevelProgressions.AddAsync(prog);
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
            }
            appInitializer.UpdateStatus("Progresiones de clase sincronizadas. Cargando reglas de experiencia, dotes y trasfondos...");
            if (data.XpRules.Any())
            {
                var existingXp = (await _unitOfWork.XpRules.GetAllAsync()).Select(x => x.Level).ToHashSet();
                var newXp = data.XpRules.Where(x => !existingXp.Contains(x.Level)).ToList();
                if (newXp.Any()) await _unitOfWork.XpRules.AddRangeAsync(newXp);
            }
            if (data.Feats.Any())
            {
                var existingFeats = (await _unitOfWork.Feats.GetAllAsync()).Select(f => f.TechnicalName.ToLower()).ToHashSet();
                var newFeats = data.Feats.Where(f => !existingFeats.Contains(f.TechnicalName.ToLower())).ToList();
                if (newFeats.Any()) await _unitOfWork.Feats.AddRangeAsync(newFeats);
            }
            if (data.Backgrounds.Any())
            {
                var existingBackgrounds = (await _unitOfWork.Backgrounds.GetAllAsync()).Select(i => i.TechnicalName.ToLower()).ToHashSet();
                var newItems = data.Backgrounds.Where(i => !existingBackgrounds.Contains(i.TechnicalName.ToLower())).ToList();
                if (newItems.Any()) await _unitOfWork.Backgrounds.AddRangeAsync(newItems);
            }
            appInitializer.UpdateStatus("Reglas de experiencia, dotes y trasfondos importados. Guardando en base de datos...");
            await _unitOfWork.SaveChangesAsync();
            appInitializer.UpdateStatus("Datos guardados. Sincronizando hechizos yobjetos");
            if (data.Spells.Any())
            {
                var existingSpells = (await _unitOfWork.Spells.GetAllAsync()).Select(s => s.TechnicalName.ToLower()).ToHashSet();
                var newSpells = data.Spells.Where(s => !existingSpells.Contains(s.TechnicalName.ToLower())).ToList();
                if (newSpells.Any()) await _unitOfWork.Spells.AddRangeAsync(newSpells);
            }
            if (data.Items.Any())
            {
                var existingItems = (await _unitOfWork.ItemTemplates.GetAllAsync()).Select(i => i.TechnicalName.ToLower()).ToHashSet();
                var newItems = data.Items.Where(i => !existingItems.Contains(i.TechnicalName.ToLower())).ToList();
                if (newItems.Any()) await _unitOfWork.ItemTemplates.AddRangeAsync(newItems);
            }
            appInitializer.UpdateStatus("Hechizos y objetos importados. Guardando en base de datos...");
            await _unitOfWork.SaveChangesAsync();
            appInitializer.UpdateStatus("Datos guardados. Sincronizando personajes...");
            if (data.Characters.Any())
            {
                var dbRaces = await _unitOfWork.Races.GetAllAsync();
                var dbClasses = await _unitOfWork.ClassDefinitions.GetAllAsync();
                var raceDict = dbRaces.ToDictionary(r => r.TechnicalName.ToLower(), r => r);
                var classDict = dbClasses.ToDictionary(c => c.TechnicalName.ToLower(), c => c);

                foreach (var character in data.Characters)
                {
                    var targetRaceName = data.Races.FirstOrDefault(r => r.Id == character.RaceId)?.TechnicalName ?? string.Empty;
                    var targetClassName = data.ClassDefinitions.FirstOrDefault(c => c.Id == character.ClassDefId)?.TechnicalName ?? string.Empty;

                    ClassDefinition? currentDbClass = null;

                    if (raceDict.TryGetValue(targetRaceName.ToLower(), out var dbRace))
                    {
                        character.RaceId = dbRace.Id;
                    }

                    if (classDict.TryGetValue(targetClassName.ToLower(), out var dbClass))
                    {
                        character.ClassDefId = dbClass.Id;

                    }

                    // Limpieza preventiva para evitar duplicados en actualizaciones
                    var existingChar = await _unitOfWork.Characters.GetByNameAsync(character.Name);
                    if (existingChar != null)
                    {
                        await _unitOfWork.Characters.RemoveAsync(existingChar);
                    }

                    if (dbClass != null && character.Level > 0)
                    {
                        var earnedProgressions = data.ClassLevelProgressions
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
                        var dbItem = await _unitOfWork.ItemTemplates.GetByNameAsync(invItem.Item.TechnicalName.Trim());
                        if (dbItem != null)
                        {
                            invItem.ItemTemplateId = dbItem.Id;
                            invItem.Item = dbItem;
                        }
                    }
                }

                await _unitOfWork.Characters.AddRangeAsync(data.Characters);
            }
            appInitializer.UpdateStatus("Personajes importados. Guardando en base de datos...");
            await _unitOfWork.SaveChangesAsync();
            appInitializer.UpdateStatus("Personajes guardados. Sincronizando contenidos localizados...");
            if (data.LocalizedContents.Any())
            {
                await _unitOfWork.LocalizedContents.AddRangeAsync(data.LocalizedContents);
            }
            appInitializer.UpdateStatus("Contenidos localizados importados. Guardando en base de datos...");
            await _unitOfWork.SaveChangesAsync();
            appInitializer.UpdateStatus("Contenidos localizados guardados. Sincronizando skills, rasgos, idiomas, subrazas, subclases y progresiones de subclase...");
            if(data.SkillProficiencies.Any())
            {
                var existingSkills = (await _unitOfWork.Skills.GetAllAsync()).Select(x => x.TechnicalName).ToHashSet();
                var newSkills = data.SkillProficiencies.Where(x => !existingSkills.Contains(x.TechnicalName)).ToList();
                if (newSkills.Any()) await _unitOfWork.Skills.AddRangeAsync(newSkills);
            }
            if (data.Traits.Any()) 
            {
                var existingTraits = (await _unitOfWork.Traits.GetAllAsync()).Select(x => x.TechnicalName).ToHashSet();
                var newTrait = data.Traits.Where(x => !existingTraits.Contains(x.TechnicalName)).ToList();
                if (newTrait.Any()) await _unitOfWork.Traits.AddRangeAsync(newTrait);
            }
            if (data.Languages.Any()) 
            {
                var existingLanguages = (await _unitOfWork.Languages.GetAllAsync()).Select(x => x.TechnicalName).ToHashSet();
                var newLanguages = data.Languages.Where(x => !existingLanguages.Contains(x.TechnicalName)).ToList();
                if (newLanguages.Any()) await _unitOfWork.Languages.AddRangeAsync(newLanguages);
            }
            if(data.SubRaces.Any())
            {
                var existingSubRaces = (await _unitOfWork.SubRaces.GetAllAsync()).Select(x => x.TechnicalName).ToHashSet();
                var newSubRaces = data.SubRaces.Where(x => !existingSubRaces.Contains(x.TechnicalName)).ToList();
                if (newSubRaces.Any()) await _unitOfWork.SubRaces.AddRangeAsync(newSubRaces);
            }
            if(data.Subclasses.Any())
            {
                var exisitnSubClases = (await _unitOfWork.Subclasses.GetAllAsync()).Select(x => x.TechnicalName).ToHashSet();
                var newSubClasses = data.Subclasses.Where(x => !exisitnSubClases.Contains(x.TechnicalName)).ToList();
                if (newSubClasses.Any()) await _unitOfWork.Subclasses.AddRangeAsync(newSubClasses);
            }
            appInitializer.UpdateStatus("Skills Rasgos, idiomas, subrazas, subclases y progresiones de subclase sincronizados. Guardando en base de datos...");

            await _unitOfWork.SaveChangesAsync();
            appInitializer.UpdateStatus("Datos guardados. Sincronización completa.");

            if (data.SubclassLevelProgressions.Any())
            {
                var dbSubProg = await _unitOfWork.SubclassLevelProgressions.GetAllAsync();
                var existingSubProgKeys = dbSubProg.Select(p => $"{p.SubclassId}_{p.Level}").ToHashSet();
                foreach (var progression in data.SubclassLevelProgressions.ToList())
                {
                    var parentSub = data.Subclasses.FirstOrDefault(c => c.Id == progression.SubclassId);
                    if (parentSub == null) continue;
                    var dbSub = await _unitOfWork.Subclasses.GetSingleAsync(x=> x.TechnicalName == parentSub.TechnicalName);
                    if (dbSub == null) continue;
                    //progression.SubclassId = dbSub.Id;
                    string key = $"{progression.SubclassId}_{progression.Level}";
                    if (!existingSubProgKeys.Contains(key))
                    {
                        await _unitOfWork.SubclassLevelProgressions.AddAsync(progression);
                        existingSubProgKeys.Add(key);
                    }
                    else
                    {
                        // Recuperamos el ID real de la DB para que los personajes puedan jalar los rasgos
                        var match = dbSubProg.First(dp => $"{dp.SubclassId}_{dp.Level}" == key);
                        progression.Id = match.Id;
                        progression.Features = match.Features; // Mantener la referencia a los rasgos de la DB
                    }
                }
            }
            appInitializer.UpdateStatus("Progresiones de subclase sincronizados. Guardando en base de datos...");
            await _unitOfWork.SaveChangesAsync();
            appInitializer.UpdateStatus("Datos guardados. Sincronización completa.");
            await _unitOfWork.CommitAsync();
            appInitializer.UpdateStatus("¡Importación finalizada! Preparando el grimorio para la aventura...");
            
            return (Count: data.Characters.Count, Version: "1.0");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}