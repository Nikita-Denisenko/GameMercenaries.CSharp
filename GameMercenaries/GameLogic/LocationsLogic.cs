using GameMercenaries.gameManagement;
using GameMercenaries.models;

namespace GameMercenaries.GameLogic;

public static class LocationsLogic
{
    private static readonly Random Random = new();
    public static Location GenerateNewLocation(Location lastLocation)
    {
        var locations = GameData.Locations;
        var newLocations = locations.Where(location => location != lastLocation).ToList();
        return newLocations[Random.Next(newLocations.Count)];
    }
    
    public static Location GenerateStartLocation()
    {
        var locations = GameData.Locations;
        return locations[Random.Next(locations.Count)];
    }
}