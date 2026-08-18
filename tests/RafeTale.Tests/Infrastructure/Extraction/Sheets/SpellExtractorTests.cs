using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class SpellExtractorTests
{
    private readonly SpellExtractor _sut = new();

    private static readonly string[] Headers =
        [ "TechnicalName", "Level", "School", "CastingTime", "Range", "RangeDistance", "Components",
          "MaterialEN", "Duration", "Concentration", "Ritual", "Classes", "NameES", "DescriptionES", "MaterialES" ];

    [Fact]
    public void Extract_ParsesAllFieldsAndClassMappings()
    {
        var wb = CreateWorkbook("Spells", Headers,
            [ "Fireball", "Level3", "Evocation", "Action", "Ranged", "150", "V,S,M", "Guano", "Instantaneous", "No", "No", "Wizard", "Bola de fuego", "Desc", "Guano" ]);
        var ctx = CreateContext();
        ctx.Package.ClassDefinitions.Add(new ClassDefinition { TechnicalName = "Wizard" });
        ctx.Package.ClassDefinitions.Add(new ClassDefinition { TechnicalName = "Cleric" });

        _sut.Extract(wb, ctx);

        var fireball = ctx.Package.Spells.Should().ContainSingle().Subject;
        fireball.Level.Should().Be(SpellLevel.Level3);
        fireball.School.Should().Be(SchoolOfMagicEnum.Evocation);
        fireball.CastingTime.Should().Be(CastingTime.Action);
        fireball.Range.Should().Be(SpellRange.Ranged);
        fireball.RangeDistance.Should().Be("150");
        fireball.Components.Should().ContainInOrder(SpellComponent.V, SpellComponent.S, SpellComponent.M);
        fireball.Ritual.Should().BeFalse();
        fireball.ClassesTechnicalNames.Should().ContainSingle("Wizard");
    }

    [Fact]
    public void Extract_AnyClass_AddsAllClasses()
    {
        var wb = CreateWorkbook("Spells", Headers,
            [ "Light", "Cantrip", "Evocation", "Action", "Touch", "", "V", "", "Instantaneous", "No", "No", "Any", "Luz", "Desc", "" ]);
        var ctx = CreateContext();
        ctx.Package.ClassDefinitions.Add(new ClassDefinition { TechnicalName = "Wizard" });
        ctx.Package.ClassDefinitions.Add(new ClassDefinition { TechnicalName = "Cleric" });

        _sut.Extract(wb, ctx);

        ctx.Package.Spells.Single().ClassesTechnicalNames.Should().BeEquivalentTo(["Wizard", "Cleric"]);
    }

    [Fact]
    public void Extract_RitualSi_ParsesAsTrue()
    {
        var wb = CreateWorkbook("Spells", Headers,
            [ "Detect", "Level1", "Divination", "Action", "Self", "", "V,S", "", "_10Minutes", "Yes", "Si", "Wizard", "Detectar", "Desc", "" ]);
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.Spells.Single().Ritual.Should().BeTrue();
    }
}