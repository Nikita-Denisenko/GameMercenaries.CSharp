using Newtonsoft.Json;

namespace GameMercenaries.Models;

public class Location
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("name")]
    public required string Name { get; init; }

    [JsonProperty("adjacent_locations")]
    public required int[] AdjacentLocations { get; init; }

    [JsonProperty("distant_locations")]
    public required int[] DistantLocations { get; init; }

    [JsonProperty("unavailable_locations")]
    public required int[] UnavailableLocations { get; init; }

    [JsonProperty("items")]
    public required Dictionary<string, int[]> Items { get; init; }

    [JsonProperty("info")]
    public required string Info { get; init; }

    [JsonProperty("rules")]
    public required Dictionary<string, int> Rules { get; init; }

    [JsonIgnore]
    public List<Player> CurrentPlayers { get; } = [];

    public void AddPlayer(Player player) => CurrentPlayers.Add(player);
    public void RemovePlayer(Player player) => CurrentPlayers.Remove(player);
    public void PrintInfo() => Console.WriteLine(Info);
}