using GameMercenaries.models.items;

namespace GameMercenaries.models;

using static GameLogic.LocationsLogic;

public class Player(
    string userName,
    Unit unit,
    Location startLocation
    )
{
    public string UserName { get; } = userName;
    public Unit Unit { get; } = unit;
    public Location Location { get; private set; } = startLocation;
    public List<Item> Inventory { get; private set; } = [];
    public int InventoryWeight { get; private set; } = 0;
    public HashSet<Artefact> Artefacts { get; private set; } = [];

    public void ChangeLocation()
    {
        Location = GenerateLocation();
    }

    public void TakeItem()
    {
        
    }
}