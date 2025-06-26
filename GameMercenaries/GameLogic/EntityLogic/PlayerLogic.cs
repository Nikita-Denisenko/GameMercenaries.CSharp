using GameMercenaries.Models;

namespace GameMercenaries.GameLogic.EntityLogic;

using gameManagement;
using static LocationsLogic;

public static class PlayerLogic
{
    private static readonly Random Random = new();
    
    public static List<Player> CreatePlayers(List<string> names)
    {
        List<Player> players = [];
        var units = new List<Unit>(GameData.Units);
        var shuffledUnits = units.OrderBy(_ => Random.Next()).ToList();
        
        for (var i = 0; i < names.Count; i++)
        {
            var location = GenerateStartLocation();
            var player = new Player(names[i], shuffledUnits[i], location);
            location.AddPlayer(player);
            players.Add(player);
        }

        return players;
    }
}