using Newtonsoft.Json;

namespace GameMercenaries.Models.Items;

public class HealthKit : Item
{
    [JsonProperty("hp")]
    public int HpBonus { get; set; }

    public HealthKit() { }
}