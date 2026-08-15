using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class ClassExtractorTests
{
    private readonly ClassExtractor _sut = new();

    private static ExtractionContext CtxWithSkills(params string[] skillNames)
    {
        var ctx = CreateContext();
        foreach (var s in skillNames)
            ctx.Package.SkillProficiencies.Add(new Skill { Id = Guid.NewGuid(), TechnicalName = s });
        return ctx;
    }

    [Fact]
    public void Extract_ParsesProficienciesAndSkills()
    {
        var wb = CreateWorkbook("Classes",
            new[] { "TechnicalName", "HitDie", "PrimaryAbility", "SavingThrows", "Armor", "Weapons", "Tools", "SkillsToChoose", "SkillList", "EquipmentES", "NameES", "DescriptionES" },
            new[] { "Wizard", "d6", "Intelligence", "Intelligence,Wisdom", "Light", "Simple", "ThievesTools", "2", "Arcana,History", "Equipo", "Mago", "Desc" });
        var ctx = CtxWithSkills("Arcana", "History");

        _sut.Extract(wb, ctx);

        var wiz = ctx.Package.ClassDefinitions.Should().ContainSingle().Subject;
        wiz.HitDie.Should().Be("d6");
        wiz.PrimaryAbility.Should().Contain(AttributeImprovementChoice.Intelligence);
        wiz.SavingThrowProficiencies.Should().Contain(AttributeImprovementChoice.Wisdom);
        wiz.ArmorProficiencies.Should().Contain(ArmorProficiency.Light);
        wiz.WeaponProficiencies.Should().Contain(WeaponProficiency.Simple);
        wiz.ToolProficiencies.Should().Contain(ToolProficiency.ThievesTools);
        wiz.SkillsToChoose.Should().Be(2);
        wiz.SkillProficiencies.Should().HaveCount(2);
    }

    [Fact]
    public void Extract_AnySkillChoice_AddsAllSkills()
    {
        var wb = CreateWorkbook("Classes",
            new[] { "TechnicalName", "HitDie", "PrimaryAbility", "SavingThrows", "Armor", "Weapons", "Tools", "SkillsToChoose", "SkillList", "EquipmentES", "NameES", "DescriptionES" },
            new[] { "Bard", "d8", "Charisma", "Dexterity,Charisma", "Light", "Simple", "", "3", "Any", "Equipo", "Bardo", "Desc" });
        var ctx = CtxWithSkills("Perception", "Stealth");

        _sut.Extract(wb, ctx);

        ctx.Package.ClassDefinitions.Single().SkillProficiencies.Should().HaveCount(2);
    }
}