using Newtonsoft.Json;
using GameMercenaries.models;
using GameMercenaries.Models.Items;
using Newtonsoft.Json.Linq;

namespace GameMercenaries.DataLoading;

public static class Loaders
{

    public static List<Location> LoadLocations()
    {
        var json = File.ReadAllText("GameMercenaries/Data/locations.json");
        return JsonConvert.DeserializeObject<List<Location>>(json) ?? [];
    }
    
    
    public static List<Unit> LoadUnits()
    {
        var json = File.ReadAllText("GameMercenaries/data/units.json");
        var units = JsonConvert.DeserializeObject<List<Unit>>(json) ?? new List<Unit>();

        return units;
    }

    public static List<Item> LoadItems()
    {
        var json = File.ReadAllText("GameMercenaries/Data/items.json");
        var rawDict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
        var items = new List<Item>();

        foreach (var (idStr, obj) in rawDict)
        {
            obj["id"] = int.Parse(idStr); // добавить ID в объект

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
    