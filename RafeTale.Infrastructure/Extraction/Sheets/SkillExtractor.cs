using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Parsing;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class SkillExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int AbilityEn = 2;
        public const int Ability = 3;
        public const int NameLoc = 4;
        public const int AbilityLoc = 5;
        public const int DescriptionLoc = 6;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        foreach (var row in workbook.GetDataRows("Skills", isRequired: true))
        {
            var skill = new Skill
            {
                Id = Guid.NewGuid(),
                TechnicalName = row.Cell(Col.TechnicalName).GetString(),
                Ability = row.Cell(Col.Ability).GetEnum<AttributeImprovementChoice>()
            };
            context.Package.SkillProficiencies.Add(skill);

            context.Localization.SaveBoth(skill.Id, LocEntity.Skill, LocProperty.Name,
                skill.TechnicalName, row.Cell(Col.NameLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(skill.Id, LocEntity.Skill, LocProperty.Ability,
                row.Cell(Col.AbilityEn).GetString(), LocLanguage.en);
            context.Localization.Save(skill.Id, LocEntity.Skill, LocProperty.Ability,
                row.Cell(Col.AbilityLoc).GetString(), context.CurrentCulture);
            context.Localization.Save(skill.Id, LocEntity.Skill, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);
        }
    }
}