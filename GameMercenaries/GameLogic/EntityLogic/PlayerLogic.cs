using GameMercenaries.Models;

namespace GameMercenaries.GameLogic;

using gameManagement;

public static class PlayerLogic
{
    private static readonly Random Random = new();
    
    public static List<Player> CreatePlayers(string[] names)
    {
        List<Player> players = [];
        var units = new List<Unit>(GameData.Units);
        var shuffledUnits = units.OrderBy(_ => Random.Next()).ToList();
        
        for (var i = 0; i < names.Length; i++)
        {
            var player = new Player(names[i], shuffledUnits[i]);
            players.Add(player);
        }

        return players;
    }
}