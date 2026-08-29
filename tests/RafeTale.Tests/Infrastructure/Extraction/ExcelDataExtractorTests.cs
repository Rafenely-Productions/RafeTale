using ClosedXML.Excel;
using FluentAssertions;
using RafeTale.Domain.Exceptions;
using RafeTale.Infrastructure.Extraction;
using RafeTale.Infrastructure.Extraction.Interfaces;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction;

public class ExcelDataExtractorTests
{
    private static XLWorkbook CreateMinimalWorkbook()
    {
        var wb = new XLWorkbook();
        AddSheet(wb, "Languages", ["TechnicalName", "DescriptionEN", "NameES", "DescriptionES"],
            [ [ "Common", "Common language", "Común", "Idioma común" ] ]);
        AddSheet(wb, "Skills", [ "TechnicalName", "AbilityEN", "Ability", "NameES", "AbilityES", "DescriptionES" ],
            [ [ "Arcana", "Intelligence", "Intelligence", "Arcanos", "Inteligencia", "Desc" ]]);
        AddSheet(wb, "Races", [ "TechnicalName", "CreatureType", "Size", "Speed", "NameES", "DescriptionES", "DescriptionEN", "Languages" ],
            [ [ "Human", "Humanoid", "Medium", "30", "Humano", "Desc", "Desc", "Common" ]]);
        AddSheet(wb, "Sub Races", [ "Race", "TechnicalName", "NameES", "Unused", "DescriptionES", "DescriptionEN" ],
            [ [ "Human", "Variant", "Variante", "", "Desc", "Desc" ]]);
        AddSheet(wb, "Traits", [ "TechnicalName", "RequiredLevel", "Race", "Subrace", "NameES", "DescriptionES", "DescriptionEN" ],
            [ [ "HumanTrait", "1", "Human", "", "Rasgo", "Desc", "Desc" ]]);
        AddSheet(wb, "Special Traits", [ "Trait", "TechnicalName", "DescriptionEN", "Modifiers", "NameES", "DescriptionES" ],
            [ [ "HumanTrait", "Special", "Desc", "[]", "Especial", "Desc" ]]);
        AddSheet(wb, "Classes", [ "TechnicalName", "HitDie", "PrimaryAbility", "SavingThrows", "Armor", "Weapons", "Tools", "SkillsToChoose", "SkillList", "EquipmentES", "NameES", "DescriptionES" ],
            [ [ "Wizard", "d6", "Intelligence", "Intelligence,Wisdom", "Light", "Simple", "", "2", "Arcana", "Equipo", "Mago", "Desc" ]]);
        AddSheet(wb, "SubClasses", [ "Class", "TechnicalName", "DescriptionEN", "NameES", "DescriptionES" ],
            [ [ "Wizard", "Evocation", "Desc", "Evocación", "Desc" ]]);
        AddSheet(wb, "Spells", [ "TechnicalName", "Level", "School", "CastingTime", "Range", "RangeDistance", "Components", "MaterialEN", "Duration", "Concentration", "Ritual", "Classes", "NameES", "DescriptionES", "MaterialES" ],
            [ [ "Fireball", "Level3", "Evocation", "Action", "Ranged", "150", "V,S,M", "Guano", "Instantaneous", "No", "No", "Wizard", "Bola de fuego", "Desc", "Guano" ]]);
        AddSheet(wb, "Feats", [ "TechnicalName", "Prerequisite", "Modifiers", "Category", "DescriptionEN", "NameES", "DescriptionES" ],
            [ [ "Tough", "none", "none", "General", "Desc", "Robusto", "Desc" ]]);
        AddSheet(wb, "Backgrounds", [ "TechnicalName", "ASIs", "Feat", "Skills", "ToolProficienciesEN", "EquipmentEN", "DescriptionEN", "NameES", "ToolProficienciesES", "EquipmentES", "DescriptionES" ],
            [ [ "Soldier", "Strength,Constitution", "Tough", "Athletics,Intimidation", "None", "Equipment", "Soldier background", "Soldado", "Ninguna", "Equipo", "Trasfondo" ]]);
        AddSheet(wb, "Personajes", [ "Name", "Race", "Class", "Level", "XP", "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" ],
            [ [ "Gandalf", "Human", "Wizard", "5", "6500", "10", "12", "14", "16", "13", "10" ]]);
        AddSheet(wb, "Items", [ "TechnicalName", "DescriptionES", "Category", "Owner", "Quantity", "IsEquipped" ],
            [ [ "Longsword", "Espada larga", "Weapon", "Gandalf", "1", "true" ]]);
        AddSheet(wb, "ClassLevelProgression", [ "Class", "Level", "ProficiencyBonus", "Feature", "ClassTraits", "Modifiers", "NameES", "DescriptionES" ],
            [ [ "Wizard", "1", "2", "Spellcasting", "SpellSlots:[4,3,2,1,0,0,0,0,0]|PreparedSpellsCount:3", "[]", "Lanzamiento", "Desc" ]]);
        AddSheet(wb, "SubClassLevelProgresion", [ "Class", "Subclass", "Feature", "Level", "Modifiers", "NameES", "DescriptionES" ],
            [ [ "Wizard", "Evocation", "SculptSpells", "3", "[]", "Esculpir", "Desc" ]]);
        AddSheet(wb, "ReglasXP", [ "Level", "RequiredXp", "Bonus" ],
            [ [ "1", "0", "0" ], [ "2", "300", "0" ]]);
        return wb;
    }

    [Fact]
    public void ExtractAllAsync_ShouldPopulateFullPackage()
    {
        var workbook = CreateMinimalWorkbook();
        using var stream = SaveToStream(workbook);

        var result = CreateFullPipeline().ExtractAllAsync(stream);

        result.Should().NotBeNull();
        result.Languages.Should().ContainSingle(l => l.TechnicalName == "Common");
        result.Races.Should().ContainSingle(r => r.TechnicalName == "Human");
        result.SkillProficiencies.Should().ContainSingle(s => s.TechnicalName == "Arcana");
        result.ClassDefinitions.Should().ContainSingle(c => c.TechnicalName == "Wizard");
        result.Subclasses.Should().ContainSingle(s => s.TechnicalName == "Evocation");
        result.Spells.Should().ContainSingle(s => s.TechnicalName == "Fireball");
        result.Feats.Should().ContainSingle(f => f.TechnicalName == "Tough");
        result.Backgrounds.Should().ContainSingle(b => b.TechnicalName == "Soldier");
        result.ClassLevelProgressions.Should().ContainSingle(p => p.Level == 1);
        result.SubclassLevelProgressions.Should().ContainSingle(p => p.Level == 3);
        result.XpRules.Should().HaveCount(2);
        result.Items.Should().ContainSingle(i => i.TechnicalName == "Longsword");

        var gandalf = result.Characters.Should().ContainSingle(c => c.Name == "Gandalf").Subject;
        gandalf.Stats["Intelligence"].Should().Be(16);

        // Order bug fixed: Items now run AFTER Characters, so the owner IS matched
        gandalf.Inventory.Should().ContainSingle(i => i.Item.TechnicalName == "Longsword");

        var wizard = result.ClassDefinitions.Single(c => c.TechnicalName == "Wizard");
        wizard.Subclasses.Should().ContainSingle(s => s.TechnicalName == "Evocation");
    }

    [Fact]
    public void ExtractAllAsync_MissingRequiredSheet_ShouldThrowDataImportException()
    {
        var workbook = CreateWorkbook("Languages",
            [ "TechnicalName", "DescriptionEN", "NameES", "DescriptionES" ],
            [ "Common", "Desc", "Común", "Desc" ]);
        using var stream = SaveToStream(workbook);

        Action act = () => CreateFullPipeline().ExtractAllAsync(stream);

        // Fails on "Skills", the first required sheet after Languages
        act.Should().Throw<DataImportException>().WithMessage("La pestaña '*' no existe en el archivo Excel.");
    }

    [Fact]
    public void ExtractAllAsync_WithSubsetPipeline_OnlyExtractsRegisteredSheets()
    {
        var workbook = CreateWorkbook("Languages",
            [ "TechnicalName", "DescriptionEN", "NameES", "DescriptionES" ],
            [ "Common", "Desc", "Común", "Desc" ]);
        using var stream = SaveToStream(workbook);

        // Only one extractor registered — no Skills sheet needed, no error
        var sut = new ExcelDataExtractor([new LanguageExtractor()]);

        var result = sut.ExtractAllAsync(stream);

        result.Languages.Should().ContainSingle(l => l.TechnicalName == "Common");
        result.Races.Should().BeEmpty();
    }
}