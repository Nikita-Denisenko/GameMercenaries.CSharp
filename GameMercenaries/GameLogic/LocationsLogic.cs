using GameMercenaries.gameManagement;
using GameMercenaries.models;

namespace GameMercenaries.GameLogic;

public static class LocationsLogic
{
    private static readonly Random Rnd = new();
    public static Location GenerateLocation()
    {
        List<Location> locations = GameData.Locations;
        var location = locations[Rnd.Next(locations.Count)];
        return location;
    }
}