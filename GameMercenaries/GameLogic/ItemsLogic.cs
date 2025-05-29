using GameMercenaries.gameManagement;
using GameMercenaries.models.items;

namespace GameMercenaries.GameLogic;

public static class ItemsLogic
{
    private static readonly Random Random = new();

    public static Item GenerateItem()
    {
        var items = GameData.Items;
        return items[Random.Next(items.Count)];
    }
}