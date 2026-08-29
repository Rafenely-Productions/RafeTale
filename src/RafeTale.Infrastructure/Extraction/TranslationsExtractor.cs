using ClosedXML.Excel;
using RafeTale.Application.Services.Importer;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;
using System;
using System.Collections.Generic;

namespace RafeTale.Infrastructure.Extraction;

public class TranslationsExtractor : ISheetExtractor
{
    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Translations", isRequired: false))
        {
            LocEntity entityTypeRaw = Enum.Parse<LocEntity>(row.Cell(1).GetString().Trim(), true); // "Attribute", "Skill", "Race", etc.
            string technicalKey = row.Cell(2).GetString().Trim().ToLowerInvariant(); // "str", "athletics", "dragonborn"
            string propertyRaw = row.Cell(3).GetString().Trim();  // "Name", "Description", "ShortName"
            LocLanguage langCode = Enum.Parse<LocLanguage>(row.Cell(4).GetString().Trim().ToLowerInvariant(), true);     // "es", "en"
            string text = row.Cell(5).GetString().Trim();

            if (string.IsNullOrWhiteSpace(text))
                continue; // Ignora stubs vacíos pendientes de traducción

            // Resolvemos el Guid de la entidad a partir de su technicalKey dentro del paquete cargado
            Guid targetEntityId = ResolveEntityGuid(context.Package, entityTypeRaw, technicalKey);

            if (targetEntityId == Guid.Empty)
                continue;

            if (Enum.TryParse<LocProperty>(propertyRaw, true, out var propertyEnum))
            {
                var locContent = new LocalizedContent
                {
                    Id = Guid.NewGuid(),
                    EntityId = targetEntityId,
                    EntityType = entityTypeRaw,
                    Property = propertyEnum,
                    LanguageCode = langCode,
                    Text = text
                };

                context.Package.LocalizedContents.Add(locContent);
            }
        }
    }

    private static Guid ResolveEntityGuid(ImportDataPackage package, LocEntity entityType, string technicalKey)
    {
        return entityType switch
        {
            LocEntity.Attribute => package.Attributes.Find(a => a.TechnicalName.Equals(technicalKey, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty,
            LocEntity.Skill => package.Skills.Find(s => s.TechnicalName.Equals(technicalKey, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty,
            LocEntity.DamageType => package.DamageTypes.Find(d => d.TechnicalName.Equals(technicalKey, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty,
            LocEntity.Condition => package.Conditions.Find(c => c.TechnicalName.Equals(technicalKey, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty,
            LocEntity.CreatureType => package.CreatureTypes.Find(ct => ct.TechnicalName.Equals(technicalKey, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty,
            LocEntity.Race => package.Races.Find(r => r.TechnicalName.Equals(technicalKey, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty,
            LocEntity.Class => package.ClassDefinitions.Find(c => c.TechnicalName.Equals(technicalKey, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty,
            _ => Guid.Empty
        };
    }
}