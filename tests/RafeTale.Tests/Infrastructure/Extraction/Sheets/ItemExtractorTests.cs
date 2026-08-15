using FluentAssertions;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Sheets;
using Xunit;
using static RafeTale.Tests.Infrastructure.Extraction.ExcelTestHelpers;

namespace RafeTale.Tests.Infrastructure.Extraction.Sheets;

public class ItemExtractorTests
{
    private readonly ItemExtractor _sut = new();

    private static readonly string[] Headers =
        { "TechnicalName", "DescriptionES", "Category", "Owner", "Quantity", "IsEquipped" };

    [Fact]
    public void Extract_AssignsInventoryToKnownOwner()
    {
        var wb = CreateWorkbook("Items", Headers,
            new[] { "Longsword", "Espada larga", "Weapon", "Gandalf", "1", "true" },
            new[] { "Potion", "Poción", "Consumable", "", "", "" });
        var ctx = CreateContext();
        var gandalf = new Character { Id = Guid.NewGuid(), Name = "Gandalf" };
        ctx.Package.Characters.Add(gandalf);

        _sut.Extract(wb, ctx);

        ctx.Package.Items.Should().HaveCount(2);
        var inv = gandalf.Inventory.Should().ContainSingle().Subject;
        inv.Item.TechnicalName.Should().Be("Longsword");
        inv.Quantity.Should().Be(1);
        inv.IsEquipped.Should().BeTrue();
    }

    [Fact]
    public void Extract_UnknownOwner_CreatesTemplateWithoutInventory()
    {
        var wb = CreateWorkbook("Items", Headers,
            new[] { "Longsword", "Espada", "Weapon", "Frodo", "1", "true" });
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.Items.Should().ContainSingle(i => i.TechnicalName == "Longsword");
    }

    [Fact]
    public void Extract_InvalidCategory_FallsBackToAdventuringGear()
    {
        var wb = CreateWorkbook("Items", Headers,
            new[] { "Thing", "Cosa", "NotACategory", "", "", "" });
        var ctx = CreateContext();

        _sut.Extract(wb, ctx);

        ctx.Package.Items.Single().Category.Should().Be(ItemCategory.AdventuringGear);
    }
}