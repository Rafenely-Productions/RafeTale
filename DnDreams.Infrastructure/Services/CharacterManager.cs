using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Services
{
    public class CharacterManager
    {
        private readonly DnDreamsDbContext _context;

        // UN SOLO CONSTRUCTOR: Inyección limpia del contexto de base de datos
        public CharacterManager(DnDreamsDbContext context)
        {
            _context = context;
        }

        // UN SOLO MÉTODO: Hace el parseo, guarda de forma transaccional y regresa una tupla con el resumen
        public async Task<(int Count, string Version)> ImportDataFromExcelAsync(Stream excelStream)
        {
            // 1. Instanciamos el servicio encargado de ClosedXML (que ya vive en Infrastructure)
            var parser = new ExcelImportService(_context);

            // 2. Parseamos el archivo Excel (asumiendo que modificaste ParseDnDExcel para retornar las listas)
            var (races, classes, characters) = parser.ParseDnDExcel(excelStream);

            // 3. Orquestamos la transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Insertar Razas que no existan
                foreach (var race in races)
                {
                    if (!_context.Races.Any(r => r.Name == race.Name))
                        _context.Races.Add(race);
                }

                // Insertar Clases que no existan
                foreach (var cls in classes)
                {
                    if (!_context.ClassDefinitions.Any(c => c.Name == cls.Name))
                        _context.ClassDefinitions.Add(cls);
                }

                await _context.SaveChangesAsync();

                // Insertar Personajes mapeando IDs
                foreach (var character in characters)
                {
                    var dbRace = _context.Races.FirstOrDefault(r => r.Name == races.First(x => x.Id == character.RaceId).Name);
                    var dbClass = _context.ClassDefinitions.FirstOrDefault(c => c.Name == classes.First(x => x.Id == character.ClassDefId).Name);

                    character.RaceId = dbRace?.Id ?? character.RaceId;
                    character.ClassDefId = dbClass?.Id ?? character.ClassDefId;

                    _context.Characters.Add(character);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 4. RETORNAMOS LA TUPLA: Datos limpios para la interfaz de usuario
                return (Count: characters.Count, Version: "1.0");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}