namespace GameMercenaries.models;

public class Location
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string[] AdjacentLocations { get; init; }
    public string[] DistantLocations { get; init; }
    public string[] UnavailableLocations { get; init; }
    public Dictionary<string, int[]> Items { get; init; }
    public string Info { get; init; }
    public Dictionary<string, int> Rules { get; init; }
    public List<Player> CurrentPlayers { get; } = [];

    public void AddPlayer(Player player) => CurrentPlayers.Add(player);
    public void RemovePlayer(Player player) => CurrentPlayers.Remove(player);
    public void PrintInfo() => Console.WriteLine(Info);
}
