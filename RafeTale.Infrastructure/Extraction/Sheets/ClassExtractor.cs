using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class ClassExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int HitDie = 2;
        public const int PrimaryAbility = 3;
        public const int SavingThrowProficiencies = 4;
        public const int ArmorProficiencies = 5;
        public const int WeaponProficiencies = 6;
        public const int ToolProficiencies = 7;
        public const int SkillsToChoose = 8;
        public const int SkillProficiencies = 9;
        public const int EquipmentLoc = 10;
        public const int NameLoc = 11;
        public const int DescriptionLoc = 12;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Classes", isRequired: true))
        {
            var classDef = new ClassDefinition
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString(),
                HitDie = row.Cell(Col.HitDie).GetString(),
                PrimaryAbility = row.Cell(Col.PrimaryAbility).GetEnumList<ASI>(),
                SavingThrowProficiencies = row.Cell(Col.SavingThrowProficiencies).GetEnumList<ASI>(),
                ArmorProficiencies = row.Cell(Col.ArmorProficiencies).GetEnumList<ArmorProficiency>(),
                WeaponProficiencies = row.Cell(Col.WeaponProficiencies).GetEnumList<WeaponProficiency>(),
                ToolProficiencies = row.Cell(Col.ToolProficiencies).GetEnumList<ToolProficiency>(),
                SkillsToChoose = row.Cell(Col.SkillsToChoose).GetValue<int>(),
            };

            MapClassSkills(classDef, row.Cell(Col.SkillProficiencies).GetString(), context.Package.SkillProficiencies);

            context.Package.ClassDefinitions.Add(classDef);

            context.Localization.Save(classDef.Id, LocEntity.Class, LocProperty.Name,
                classDef.TechnicalName, LocLanguage.en);
            context.Localization.Save(classDef.Id, LocEntity.Class, LocProperty.Equipment,
                row.Cell(Col.EquipmentLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(classDef.Id, LocEntity.Class, LocProperty.Name,
                row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(classDef.Id, LocEntity.Class, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }

    private void MapClassSkills(ClassDefinition classDef, string rawSkills, List<Skill> allSkills)
    {
        var skillNames = rawSkills.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
        if (skillNames.Contains("Any", StringComparer.OrdinalIgnoreCase))
        {
            classDef.SkillProficiencies.AddRange(allSkills);
            return;
        }

        foreach (var name in skillNames)
        {
            var matched = allSkills.FirstOrDefault(s => s.TechnicalName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (matched != null) classDef.SkillProficiencies.Add(matched);
        }
    }
}