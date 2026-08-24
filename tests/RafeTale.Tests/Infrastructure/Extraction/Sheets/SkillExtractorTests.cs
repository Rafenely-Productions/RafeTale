using FluentAssertions;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class SkillExtractorTests
{
    private readonly SkillExtractor _sut = new();

    [Fact]
    public void Extract_ParsesAbility()
    {
        var wb = CreateWorkbook("Skills",
            ["TechnicalName", "AbilityEN", "Ability", "NameES", "AbilityES", "DescriptionES"],
            ["Perception", "Wisdom", "Wisdom", "Percepción", "Sabiduría", "Desc"]);
        var context = CreateContext();

        _sut.Extract(wb, context);

        var skill = context.Package.SkillProficiencies.Should().ContainSingle().Subject;
        skill.TechnicalName.Should().Be("Perception");
        skill.Ability.Should().Be(AttributeImprovementChoice.Wisdom);
    }

    [Fact]
    public void Extract_InvalidAbility_FallsBackToDefault()
    {
        var wb = CreateWorkbook("Skills",
            ["TechnicalName", "AbilityEN", "Ability", "NameES", "AbilityES", "DescriptionES"],
            ["Broken", "?", "NotAnAbility", "Rota", "?", "Desc"]);
        var context = CreateContext();

        _sut.Extract(wb, context);

        context.Package.SkillProficiencies.Single().Ability.Should().Be(default);
    }
}