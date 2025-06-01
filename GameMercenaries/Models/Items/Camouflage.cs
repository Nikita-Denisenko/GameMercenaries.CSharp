using Newtonsoft.Json;

namespace GameMercenaries.Models.Items;

public class Camouflage : Item
{
    [JsonProperty("cut_enemy_accuracy")]
    public int CutEnemyAccuracyBonus { get; set; }
}