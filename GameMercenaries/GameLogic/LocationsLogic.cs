using GameMercenaries.gameManagement;
using GameMercenaries.models;

namespace GameMercenaries.GameLogic;

public static class LocationsLogic
{
    private static readonly Random Rnd = new();
    public static Location GenerateLocation(Location? lastLocation = null)
    {
        var locations = GameData.Locations;
        if (lastLocation is null) return locations[Rnd.Next(locations.Count)];
        var newLocations = locations.Where(location => location != lastLocation).ToList();
        return newLocations[Rnd.Next(newLocations.Count)];
    }
}