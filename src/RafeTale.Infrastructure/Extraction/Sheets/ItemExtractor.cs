using ClosedXML.Excel;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Infrastructure.Extraction.Extensions;
using RafeTale.Infrastructure.Extraction.Interfaces;

namespace RafeTale.Infrastructure.Extraction.Sheets;

public class ItemExtractor : ISheetExtractor
{
    private static class Col
    {
        public const int TechnicalName = 1;
        public const int DescriptionLoc = 2;
        public const int Category = 3;
        public const int OwnerName = 4;
        public const int Quantity = 5;
        public const int IsEquipped = 6;
    }

    public void Extract(IXLWorkbook workbook, ExtractionContext context)
    {
        var sheet = workbook.GetSheetSafe("Items");
        if (sheet == null) return;

        var lastCol = sheet.LastColumnUsed()?.ColumnNumber() ?? 0;

        foreach (var row in sheet.RangeUsed()?.RowsUsed().Skip(1) ?? [])
        {
            var itemName = row.Cell(Col.TechnicalName).GetString();
            if (string.IsNullOrWhiteSpace(itemName)) continue;

            var template = new ItemTemplate
            {
                Id = Guid.NewGuid(),
                TechnicalName = itemName,
                Category = lastCol >= Col.Category && Enum.TryParse<ItemCategory>(row.Cell(Col.Category).GetString(), true, out var cat)
                    ? cat
                    : ItemCategory.AdventuringGear
            };

            context.Package.Items.Add(template);

            context.Localization.Save(template.Id, LocEntity.ItemTemplate, LocProperty.Name, itemName, context.CurrentCulture);
            context.Localization.Save(template.Id, LocEntity.ItemTemplate, LocProperty.Description,
                row.Cell(Col.DescriptionLoc).GetString(), context.CurrentCulture);

            // Owner linking — Characters MUST already be extracted
            if (lastCol >= Col.OwnerName)
            {
                var ownerName = row.Cell(Col.OwnerName).GetString();
                var owner = context.Package.Characters
                    .FirstOrDefault(c => c.Name.Equals(ownerName, StringComparison.OrdinalIgnoreCase));

                if (owner != null)
                {
                    var qty = lastCol >= Col.Quantity ? row.Cell(Col.Quantity).GetValue<int>() : 1;
                    var equipped = lastCol >= Col.IsEquipped && row.Cell(Col.IsEquipped).GetValue<bool>();

                    owner.Inventory.Add(new CharacterInventory
                    {
                        Id = Guid.NewGuid(),
                        CharacterId = owner.Id,
                        ItemTemplateId = template.Id,
                        Item = template,
                        Quantity = qty <= 0 ? 1 : qty,
                        IsEquipped = equipped
                    });
                }
            }
        }
    }
}