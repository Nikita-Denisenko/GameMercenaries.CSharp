using Newtonsoft.Json;
using GameMercenaries.models;
using GameMercenaries.models.items;
using Newtonsoft.Json.Linq;

namespace GameMercenaries.DataLoading;

public static class Loaders
{
    public static List<Location> LoadLocations()
    {
        var json = File.ReadAllText("GameMercenaries/data/locations.json");
        var locations = JsonConvert.DeserializeObject<List<Location>>(json) ?? [];
        return locations;
    }

    public static List<Unit> LoadUnits()
    {
        var json = File.ReadAllText("GameMercenaries/data/units.json");
        var units = JsonConvert.DeserializeObject<List<Unit>>(json) ?? [];
        return units;
    }

    public static List<Item> LoadItems()
    {
        List <Item> resultItems = [];
        var json = File.ReadAllText("GameMercenaries/data/items.json");
        var itemsObj = JObject.Parse(json);

        foreach (var property in itemsObj.Properties())
        {
            var id = property.Name;
            var itemData = property.Value;
            
            var name = itemData["name"]?.ToString() ?? "";
            var itemType = itemData["item_type"]?.ToString() ?? "";
            var maxQuantity = itemData["quantity"]?.ToObject<int>() ?? 0;
            var currentQuantity = itemData["currentQuantity"]?.ToObject<int>() ?? 0;
            var weight = itemData["weight"]?.ToObject<int>() ?? 0;
            var info = itemData["info"]?.ToString() ?? "";
            
            switch (itemType)
            {
                case "Артефакт":
                    resultItems.Add(new Artefact(id, name, itemType, maxQuantity, currentQuantity, weight, info));
                    break;
                case  "Броня":
                    var damageReduction = itemData["damage_reduction"]?.ToObject<int>() ?? 0;
                    resultItems.Add(new Armor(id, name, itemType, maxQuantity, currentQuantity, weight,  info,  damageReduction));
                    break;
                case "Маскировочное снаряжение":
                    var cutEnemyAccuracy = itemData["cut_enemy_accuracy"]?.ToObject<int>() ?? 0;
                    resultItems.Add(new Camouflage(id, name, itemType, maxQuantity, currentQuantity, weight, info, cutEnemyAccuracy));
                    break;
                case "Аптечка":
                    var hpBonus = itemData["hp"]?.ToObject<int>() ?? 0;
                    resultItems.Add(new HealthKit(id, name, itemType, maxQuantity, currentQuantity, weight, info, hpBonus));
                    break;
                case "Оружие":
                    var weaponType = itemData["weapon_type"]?.ToString() ?? "";
                    var damageRange = itemData["damage_range"]?.ToObject<int[]>() ?? [];
                    var distance = itemData["distance"]?.ToObject<int>() ?? 0;
                    var accuracy = itemData["accuracy"]?.ToObject<int>() ?? 0;
                    var rules = itemData["rules"]?.ToObject<Dictionary<string, int>>() ?? new Dictionary<string, int>();
                    resultItems.Add(new Weapon(id, name, itemType, maxQuantity, currentQuantity, weight, info, 
                                    weaponType, damageRange, distance, accuracy, rules));
                    break;
                case "Бустер на оружие":
                    var accuracyBonus = itemData["accuracy_bonus"]?.ToObject<int>() ?? 0;
                    resultItems.Add(new WeaponUpgrade(id, name, itemType, maxQuantity, currentQuantity, weight, info, accuracyBonus));
                    break;
            }
        }
        return resultItems;
    }
}
    