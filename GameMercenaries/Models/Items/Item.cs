using Newtonsoft.Json;

namespace GameMercenaries.Models.Items;

public abstract class Item
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("item_type")]
    public string ItemType { get; set; } = string.Empty;

    [JsonProperty("weight")]
    public int Weight { get; set; }

    [JsonProperty("info")]
    public string Info { get; set; } = string.Empty;
    

    public virtual void PrintInfo()
    {
        Console.WriteLine($"{Name} ({ItemType}) — {Info}");
    }
}