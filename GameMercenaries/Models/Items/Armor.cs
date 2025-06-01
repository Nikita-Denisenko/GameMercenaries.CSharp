using Newtonsoft.Json;

namespace GameMercenaries.Models.Items;

public class Armor : Item
{
    [JsonProperty("damage_reduction")]
    public int DamageReduction { get; set; }
}