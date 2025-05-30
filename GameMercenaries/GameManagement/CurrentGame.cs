using GameMercenaries.models;
using GameMercenaries.models.items;

namespace GameMercenaries.gameManagement;

public class CurrentGame(
    List<Player> players)
{
    public List<Location> Locations { get; } = GameData.Locations;
    public List<Unit> Units { get; } = GameData.Units;
    public List<Item> Items { get; } =  GameData.Items;
    public List<Player> AllPlayers { get; } = players;
    public List<Player> AlivePlayers { get; } = players;
    public List<string> GameEvents { get; } = [];

    public void KillPlayer(Player player)
    {
        AlivePlayers.Remove(player);
    }

    public void AddEvent(string eventText)
    {
        GameEvents.Add(eventText);
    }
}