namespace GameMercenaries.models;

public class Location(
    string id,
    string name,
    string[] adjacentLocations,
    string[] distantLocations,
    string[] unavailableLocations,
    Dictionary<string, int[]> items,
    string info,
    Dictionary<string, int> rules
    )
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string[] AdjacentLocations { get; } = adjacentLocations;
    public string[] DistantLocations { get; } = distantLocations;
    public string[] UnavailableLocations { get; } = unavailableLocations;
    public Dictionary<string, int[]> Items { get; } = items;
    public Dictionary<string, int> Rules { get; } = rules;
    public List<Player> CurrentPlayers { get; } = [];
    
    public void AddPlayer(Player player)
    {
        CurrentPlayers.Add(player);
    }
    
    public void RemovePlayer(Player player)
    {
        CurrentPlayers.Remove(player);
    }

    public void PrintInfo() => Console.WriteLine(info);
}
