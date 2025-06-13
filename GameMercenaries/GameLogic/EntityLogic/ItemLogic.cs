using GameMercenaries.gameManagement;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;

namespace GameMercenaries.GameLogic;

public static class ItemLogic
{
    private static readonly Random Random = new();

    public static Item GenerateStartItem()
    {
        var items = GameData.Items;
        return items[Random.Next(items.Count)];
    }
    
    public static Item GenerateItem(Location location)
    {
        var itemsRanges = location.Items;
        var number = Random.Next(1, 101);
        foreach (var (item, range)  in itemsRanges)
        {
            if (range[0] <= number && number <= range[1])
            { 
                return GameData.Items[int.Parse(item) - 1];
            }
        }
        throw new InvalidOperationException("Не удалось сгенерировать предмет: диапазон вероятностей не покрывает число.");
    }
}