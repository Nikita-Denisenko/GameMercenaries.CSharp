using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;

namespace GameMercenaries.DataLoading;

public static class Loaders
{
    public static List<Location> LoadLocations()
    {
        var json = File.ReadAllText("D:\\С# Projects\\GameMercenaries\\GameMercenaries\\Data\\locations.json");
        return JsonConvert.DeserializeObject<List<Location>>(json) ?? new List<Location>();
    }

    public static List<Unit> LoadUnits()
    {
        var json = File.ReadAllText("D:\\С# Projects\\GameMercenaries\\GameMercenaries\\Data\\units.json");
        return JsonConvert.DeserializeObject<List<Unit>>(json) ?? new List<Unit>();
    }

    public static List<Item> LoadItems()
    {
        var json = File.ReadAllText("D:\\С# Projects\\GameMercenaries\\GameMercenaries\\Data\\items.json");
        var rawList = JsonConvert.DeserializeObject<List<JObject>>(json) ?? new List<JObject>();
        var items = new List<Item>();

        foreach (var obj in rawList)
        {
            var type = obj["item_type"]?.ToString();

            Item? item = type switch
            {
                "Оружие" => obj.ToObject<Weapon>(),
                "Артефакт" => obj.ToObject<Artefact>(),
                "Броня" => obj.ToObject<Armor>(),
                "Маскировочное снаряжение" => obj.ToObject<Camouflage>(),
                "Бустер на оружие" => obj.ToObject<WeaponUpgrade>(),
                "Медикамент" => obj.ToObject<HealthKit>(),
                _ => null
            };

            if (item != null)
                items.Add(item);
        }

        return items;
    }
}