using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class SpellExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int Level = 2;
        public const int School = 3;
        public const int CastingTime = 4;
        public const int Range = 5;
        public const int RangeDistance = 6;
        public const int Components = 7;
        public const int MaterialComponentDescEn = 8;
        public const int Duration = 9;
        public const int Concentration = 10;
        public const int Ritual = 11;
        public const int Classes = 12;
        public const int DescriptionEn = 12; // Same column as classes in original
        public const int NameLoc = 13;
        public const int DescriptionLoc = 14;
        public const int MaterialComponentDescLoc = 15;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        var classTechNames = context.Package.ClassDefinitions.Select(x => x.TechnicalName).ToList();

        foreach (var row in workbook.GetDataRows("Spells", isRequired: true))
        {
            var spell = new Spell
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString() ?? string.Empty,
                Level = row.Cell(Col.Level).GetEnum<SpellLevel>(),
                School = row.Cell(Col.School).GetEnum<SchoolOfMagicEnum>(),
                CastingTime = row.Cell(Col.CastingTime).GetEnum<CastingTime>(),
                Range = row.Cell(Col.Range).GetEnum<SpellRange>(),
                RangeDistance = row.Cell(Col.RangeDistance).GetValue<string>(),
                Components = row.Cell(Col.Components).GetEnumList<SpellComponent>(),
                Duration = row.Cell(Col.Duration).GetEnumList<SpellDuration>(),
                Concentration = row.Cell(Col.Concentration).GetEnum<SpellConcentration>(),
                Ritual = row.Cell(Col.Ritual).GetString().Equals("Si", StringComparison.OrdinalIgnoreCase),
            };

            MapSpellClass(spell, row.Cell(Col.Classes).GetString(), classTechNames);

            context.Package.Spells.Add(spell);

            context.Localization.Save(spell.Id, LocEntity.Spell, LocProperty.Name,
                spell.TechnicalName, LocLanguage.en);
            context.Localization.Save(spell.Id, LocEntity.Spell, LocProperty.Description,
                row.Cell(Col.DescriptionEn).GetString(), LocLanguage.en);
            context.Localization.Save(spell.Id, LocEntity.Spell, LocProperty.MaterialComponentDescription,
                row.Cell(Col.MaterialComponentDescEn).GetString(), LocLanguage.en);
            context.Localization.Save(spell.Id, LocEntity.Spell, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(spell.Id, LocEntity.Spell, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(spell.Id, LocEntity.Spell, LocProperty.MaterialComponentDescription,
                row.Cell(Col.MaterialComponentDescLoc).GetString(), context.CurrentCulture);
        }
    }

    private void MapSpellClass(Spell spell, string rawClass, List<string> allClasses)
    {
        var classNames = rawClass.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
        if (classNames.Contains("Any", StringComparer.OrdinalIgnoreCase))
        {
            spell.ClassesTechnicalNames.AddRange(allClasses);
            return;
        }

        foreach (var name in classNames)
        {
            var matched = allClasses.FirstOrDefault(s => s.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (matched != null) spell.ClassesTechnicalNames.Add(matched);
        }
    }
}