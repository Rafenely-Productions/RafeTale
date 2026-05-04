using DnDreams.Application.Models;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services
{
    public class CharacterManager
    {
        private readonly ICharacterRepository _repository;
        private readonly ExcelDataService _excelService;

        public CharacterManager(ICharacterRepository repository, ExcelDataService excelService)
        {
            _repository = repository;
            _excelService = excelService;
        }

        public async Task<ImportSummary> ImportFromExcelAsync(Stream fileStream)
        {
            // 1. Convertir Excel a Entidades de Domain
            var importResult = _excelService.ImportCharactersFromExcel(fileStream);

            // 2. Aquí podrías validar versiones antes de guardar
            // if (importResult.Version != "1.0") { ... }

            // 3. Guardado Masivo
            await _repository.BulkInsertCharactersAsync(importResult.Data);

            return new ImportSummary
            {
                Count = importResult.Data.Count,
                Version = importResult.Version
            };
        }
    }
}
