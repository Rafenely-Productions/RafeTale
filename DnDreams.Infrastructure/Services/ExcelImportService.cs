using ClosedXML.Excel;
using DnDreams.Domain.Entities;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Services
{
    public class ExcelImportService
    {
        private readonly DnDreamsDbContext _context;
        
        public ExcelImportService(DnDreamsDbContext context) => _context = context;
        
        public async Task<(int Count, string Version)> ImportFromExcelAsync(Stream excelStream)
        {
            var racesList = new List<Race>();
            var classesList = new List<ClassDefinition>();
            var charactersList = new List<Character>();

            using var workbook = new XLWorkbook(excelStream);

            // 1. LEER PESTAÑA DE RAZAS
            if (workbook.TryGetWorksheet("Razas", out var raceSheet))
            {
                foreach (var row in raceSheet.RangeUsed().RowsUsed().Skip(1))
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
                foreach (var row in classSheet.RangeUsed().RowsUsed().Skip(1))
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
                foreach (var row in charSheet.RangeUsed().RowsUsed().Skip(1))
                {
                    var raceName = row.Cell(2).GetString();
                    var className = row.Cell(3).GetString();

                    var matchedRace = racesList.FirstOrDefault(r => r.Name.Equals(raceName, StringComparison.OrdinalIgnoreCase));
                    var matchedClass = classesList.FirstOrDefault(c => c.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

                    var character = new Character
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(1).GetString(),
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

            // 4. GUARDAR EN BASE DE DATOS USANDO TRANSACCIÓN
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var race in racesList)
                {
                    if (!await _context.Races.AnyAsync(r => r.Name == race.Name))
                        _context.Races.Add(race);
                }

                foreach (var cls in classesList)
                {
                    if (!await _context.ClassDefinitions.AnyAsync(c => c.Name == cls.Name))
                        _context.ClassDefinitions.Add(cls);
                }

                await _context.SaveChangesAsync();

                foreach (var character in charactersList)
                {
                    var targetRaceName = racesList.First(x => x.Id == character.RaceId).Name;
                    var targetClassName = classesList.First(x => x.Id == character.ClassDefId).Name;

                    var dbRace = await _context.Races.FirstOrDefaultAsync(r => r.Name == targetRaceName);
                    var dbClass = await _context.ClassDefinitions.FirstOrDefaultAsync(c => c.Name == targetClassName);

                    character.RaceId = dbRace?.Id ?? character.RaceId;
                    character.ClassDefId = dbClass?.Id ?? character.ClassDefId;

                    _context.Characters.Add(character);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (Count: 10, Version: "1.0");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public (List<Race> Races, List<ClassDefinition> Classes, List<Character> Characters) ParseDnDExcel(Stream excelStream)
        {
            var racesList = new List<Race>();
            var classesList = new List<ClassDefinition>();
            var charactersList = new List<Character>();

            using var workbook = new XLWorkbook(excelStream);

            // 1. LEER PESTAÑA DE RAZAS
            if (workbook.TryGetWorksheet("Razas", out var raceSheet))
            {
                var rows = raceSheet.RangeUsed().RowsUsed().Skip(1); // Saltar encabezados
                foreach (var row in rows)
                {
                    var race = new Race
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(1).GetString(),
                        Speed = row.Cell(2).GetValue<int>()
                    };

                    // Leer columnas de bonos de estadísticas dinámicamente (Fuerza, Destreza, etc.)
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
                    var classDef = new ClassDefinition
                    {
                        Id = Guid.NewGuid(),
                        Name = row.Cell(1).GetString(),
                        HitDie = row.Cell(2).GetString()
                    };
                    classesList.Add(classDef);
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

                    // Buscamos las referencias de los objetos creados arriba para asociar los IDs correspondientes
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

                    // Mapear los Stats base del personaje a su diccionario JSON (Fuerza, Destreza, etc.)
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

            return (racesList, classesList, charactersList);
        }
    }
}
