using GameMercenaries.gameManagement;
using GameMercenaries.models.items;

namespace GameMercenaries.GameLogic;

public static class ItemsLogic
{
    private static readonly Random Rnd = new();

    public static Item? GenerateItem()
    {
        var items = GameData.Items.Where(item => item.CurrentQuantity > 0).ToList();
        if (items.Count == 0) return null;
        var item = items[Rnd.Next(items.Count)];
        
        return item;
    }
    
    public static List<Item> GenerateStartItems()
    {
        const int maxQuantityStartItems = 4;
        List<Item> startItems = [];
        var quantityStartItems = Rnd.Next(1, maxQuantityStartItems + 1);
        
        for (var i = 0; i < quantityStartItems; i++)
        {
            var item = GenerateItem();
            if (item != null) startItems.Add(item);
        }

        return startItems;
    }
}