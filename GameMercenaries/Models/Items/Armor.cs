using Newtonsoft.Json;

namespace GameMercenaries.Models.Items;

public class Armor : Item
{
    [JsonProperty("damage_reduction")]
    public int DamageReduction { get; set; }
    [JsonProperty("break_chance")]
    public int BreakChance { get; set; }
}