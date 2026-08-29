using ClosedXML.Excel;
using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Entities.Rules;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction;

public class RulebookExtractor : ISheetExtractor
{
    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        // 1. Extraer Metadatos del Libro (BookInfo)
        var bookInfoSheet = workbook.Worksheet("BookInfo");
        if (bookInfoSheet != null)
        {
            string suppLangsRaw = bookInfoSheet.Cell("G2").GetString().Trim();
            string defaultLang = bookInfoSheet.Cell("H2").GetString().Trim();

            var suppLangsList = string.IsNullOrWhiteSpace(suppLangsRaw)
                ? ["es", "en"]
                : suppLangsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(l => l.Trim().ToLowerInvariant())
                              .ToList();

            var rulebook = new Rulebook
            {
                Id = Guid.NewGuid(),
                BookId = bookInfoSheet.Cell("A2").GetString().Trim().ToLowerInvariant(),
                SystemId = bookInfoSheet.Cell("B2").GetString().Trim(),
                Title = bookInfoSheet.Cell("C2").GetString().Trim(),
                Type = bookInfoSheet.Cell("D2").GetString().Trim(),
                Version = bookInfoSheet.Cell("E2").GetString().Trim(),
                Author = bookInfoSheet.Cell("F2").GetString().Trim(),
                SupportedLanguages = suppLangsList,
                DefaultLanguage = string.IsNullOrWhiteSpace(defaultLang) ? "es" : defaultLang.ToLowerInvariant(),

                Notes = bookInfoSheet.Cell("B4").GetString().Trim()
            };
            context.Package.Rulebook = rulebook;
        }

        Guid rulebookId = context.Package.Rulebook?.Id ?? Guid.NewGuid();

        // 2. Extraer Atributos (Attributes)
        foreach (var row in workbook.GetDataRows("Attributes", isRequired: true))
        {
            var attr = new AttributeDefinition
            {
                Id = Guid.NewGuid(),
                RulebookId = rulebookId,
                TechnicalName = row.Cell(1).GetString().Trim().ToLowerInvariant(),
                DefaultMin = row.Cell(2).GetValue<int>(),
                DefaultMax = row.Cell(3).GetValue<int>(),
                DisplayOrder = row.Cell(4).GetValue<int>()
            };
            context.Package.Attributes.Add(attr);
        }

        // 3. Extraer Habilidades (Skills)
        foreach (var row in workbook.GetDataRows("Skills", isRequired: true))
        {
            string skillKey = row.Cell(1).GetString().Trim().ToLowerInvariant();
            string targetAttrKey = row.Cell(2).GetString().Trim().ToLowerInvariant(); // ej: "str"

            // Buscamos el Guid del atributo correspondiente
            var parentAttr = context.Package.Attributes.FirstOrDefault(a => a.TechnicalName == targetAttrKey);

            var skill = new SkillDefinition
            {
                Id = Guid.NewGuid(),
                RulebookId = rulebookId,
                TechnicalName = skillKey,
                AttributeId = parentAttr?.Id ?? Guid.Empty // O asignar la clave foránea encontrada
            };
            context.Package.Skills.Add(skill);
        }

        // 4. Extraer Tipos de Daño (DamageTypes)
        foreach (var row in workbook.GetDataRows("DamageTypes", isRequired: false))
        {
            var damage = new DamageTypeDefinition
            {
                Id = Guid.NewGuid(),
                RulebookId = rulebookId,
                TechnicalName = row.Cell(1).GetString().Trim().ToLowerInvariant(),
            };
            context.Package.DamageTypes.Add(damage);
        }

        // 5. Extraer Condiciones / Estados (Conditions)
        foreach (var row in workbook.GetDataRows("Conditions", isRequired: false))
        {
            var cond = new ConditionDefinition
            {
                Id = Guid.NewGuid(),
                RulebookId = rulebookId,
                TechnicalName = row.Cell(1).GetString().Trim().ToLowerInvariant(),
            };
            context.Package.Conditions.Add(cond);
        }

        // 6. Extraer Tipos de Criatura (CreatureTypes)
        foreach (var row in workbook.GetDataRows("CreatureTypes", isRequired: false))
        {
            var creature = new CreatureTypeDefinition
            {
                Id = Guid.NewGuid(),
                RulebookId = rulebookId,
                TechnicalName = row.Cell(1).GetString().Trim().ToLowerInvariant(),
            };
            context.Package.CreatureTypes.Add(creature);
        }
    }
}
