using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class BackgroundExtractorTests
{
    private readonly BackgroundExtractor _sut = new();

    [Fact]
    public void Extract_LinksFeatAndParsesProficiencies()
    {
        var wb = CreateWorkbook("Backgrounds",
            [ "TechnicalName", "ASIs", "Feat", "Skills", "ToolProficienciesEN", "EquipmentEN", "DescriptionEN", "NameES", "ToolProficienciesES", "EquipmentES", "DescriptionES" ],
            [ "Soldier", "Strength,Constitution", "Tough", "Athletics,Intimidation", "None", "Equipment", "Desc", "Soldado", "Ninguna", "Equipo", "Trasfondo" ]);
        var ctx = CreateContext();
        var feat = new Feat { Id = Guid.NewGuid(), TechnicalName = "Tough" };
        ctx.Package.Feats.Add(feat);

        _sut.Extract(wb, ctx);

        var bg = ctx.Package.Backgrounds.Should().ContainSingle().Subject;
        bg.FeatId.Should().Be(feat.Id);
        bg.Feat.Should().Be(feat);
        bg.ASIs.Should().BeEquivalentTo([ AttributeImprovementChoice.Strength, AttributeImprovementChoice.Constitution ]);
        bg.SkillProficiencies.Should().BeEquivalentTo([ SkillType.Athletics, SkillType.Intimidation ]);
    }

    [Fact]
    public void Extract_UnknownFeat_LeavesFeatIdEmpty()
    {
        var wb = CreateWorkbook("Backgrounds",
            [ "TechnicalName", "ASIs", "Feat", "Skills", "ToolProficienciesEN", "EquipmentEN", "DescriptionEN", "NameES", "ToolProficienciesES", "EquipmentES", "DescriptionES" ],
            [ "Hermit", "Wisdom", "NonExistent", "Medicine", "", "", "", "Ermitaño", "", "", "" ]);
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.Backgrounds.Single().FeatId.Should().Be(Guid.Empty);
    }
}