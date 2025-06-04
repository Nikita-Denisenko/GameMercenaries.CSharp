using Newtonsoft.Json;

namespace GameMercenaries.Models.Items;

public class Weapon : Item
{
    [JsonProperty("weapon_type")]
    public string WeaponType { get; set; } = string.Empty;

    [JsonProperty("damage_range")]
    public int[] DamageRange { get; set; } = [];

    [JsonProperty("distance")]
    public int Distance { get; set; }

    [JsonProperty("accuracy")]
    public int Accuracy { get; set; }

    [JsonProperty("rules")]
    public Dictionary<string, int> Rules { get; set; } = new();

    public int GenerateDamage()
    {
        Random random = new();
        return random.Next(DamageRange[0], DamageRange[1] + 1);
    }
}