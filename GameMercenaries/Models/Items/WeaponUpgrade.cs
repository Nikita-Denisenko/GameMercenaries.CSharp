using Newtonsoft.Json;

namespace GameMercenaries.Models.Items;

public class WeaponUpgrade : Item
{
    [JsonProperty("accuracy_bonus")]
    public int AccuracyBonus { get; set; }
}