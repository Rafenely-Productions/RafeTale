using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class CharacterExtractor : ISheetExtractor
{
    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        var sheet = workbook.GetSheetSafe("Personajes");
        if (sheet == null) return;

        // Read dynamic stat columns from header row
        var headerRow = sheet.FirstRowUsed();
        var lastCol = sheet.LastColumnUsed()?.ColumnNumber() ?? 0;
        var statColumns = new Dictionary<int, string>(); // colIndex -> statName

        if (headerRow != null)
        {
            for (int c = 6; c <= lastCol; c++)
            {
                var name = headerRow.Cell(c).GetString();
                if (!string.IsNullOrEmpty(name)) statColumns[c] = name;
            }
        }

        // Safe default background instead of backgrounds[0]
        var defaultBackground = context.Package.Backgrounds.FirstOrDefault();

        foreach (var row in sheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>())
        {
            var charName = row.Cell(1).GetString();
            var raceName = row.Cell(2).GetString();
            var className = row.Cell(3).GetString();

            var race = context.Package.Races
                .FirstOrDefault(r => r.TechnicalName.Equals(raceName, StringComparison.OrdinalIgnoreCase));
            var classDef = context.Package.ClassDefinitions
                .FirstOrDefault(c => c.TechnicalName.Equals(className, StringComparison.OrdinalIgnoreCase));

            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = charName,
                Level = row.Cell(4).GetValue<int>(),
                Experience = row.Cell(5).GetValue<int>(),
                RaceId = race?.Id ?? Guid.Empty,
                ClassDefId = classDef?.Id ?? Guid.Empty,
                BackgroundId = defaultBackground?.Id ?? Guid.Empty,
                Background = defaultBackground!,
                AcquiredFeats = new List<Feat>(),
                Stats = new Dictionary<string, int>(),
                AcquiredFeatures = new List<Feature>(),
                ActiveModifiers = new List<ActiveModifiers>(),
            };

            foreach (var (col, statName) in statColumns)
            {
                var val = row.Cell(col).GetValue<int>();
                character.Stats[statName] = val;
            }

            context.Package.Characters.Add(character);
        }
    }
}