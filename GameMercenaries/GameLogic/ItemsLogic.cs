using GameMercenaries.gameManagement;
using GameMercenaries.models.items;

namespace GameMercenaries.GameLogic;

public static class ItemsLogic
{
    private static readonly Random Rnd = new();

    public static Item GenerateItem()
    {
        var items = GameData.Items;
        return items[Rnd.Next(items.Count)];;
    }
}