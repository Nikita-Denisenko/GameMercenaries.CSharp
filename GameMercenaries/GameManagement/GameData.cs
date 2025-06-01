using GameMercenaries.Models;
using GameMercenaries.Models.Items;
using static GameMercenaries.DataLoading.Loaders;

namespace GameMercenaries.gameManagement;

public static class GameData
{
    public static List<Location> Locations { get; private set; }
    public static List<Unit> Units { get; private set; }
    public static List<Item> Items { get; private set; }

    static GameData()
    {
        Locations = LoadLocations();
        Units = LoadUnits();
        Items = LoadItems();
    }
}