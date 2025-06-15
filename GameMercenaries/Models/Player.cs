using GameMercenaries.Models.Items;

namespace GameMercenaries.Models;

using static GameLogic.LocationsLogic;
using static UserInterface.UserInterface;
using static GameLogic.ItemLogic;


public class Player(
    string userName,
    Unit unit
    )
{
    public string UserName { get; } = userName;
    public Unit Unit { get; } = unit;
    public Location Location { get; private set; } = GenerateStartLocation();
    public Item ItemOnLocation { get; private set; } = GenerateStartItem();
    public bool ItemWasTaken { get; private set; } = false;
    public List<Item> Inventory { get; } = [];
    public int InventoryWeight { get; private set; }
    public HashSet<Artefact> Artefacts { get; private set; } = [];
    
    public void ChangeLocation()
    {
        Location = GenerateNewLocation(Location);
        ItemOnLocation = GenerateItem(Location);
        ItemWasTaken = false;
    }

    public bool CanTakeItem(Item item)
    {
        return Unit.Weight >= InventoryWeight + item.Weight;
    }
    
    public void TakeItem(Item item)
    {
        Inventory.Add(item);
        InventoryWeight += item.Weight;
    }

    public void FindItem()
    {
        if (!CanTakeItem(ItemOnLocation))
        {
            Console.WriteLine("Недостаточно места в инвентаре для этого предмета!");
            return;
        }
        
        TakeItem(ItemOnLocation);
        
        ItemWasTaken = true;
        
        Console.WriteLine($"Вы подобрали предмет {ItemOnLocation.Name}");
    }

    public void RemoveItem()
    {
        Console.WriteLine("Ваш инвентарь:");
        PrintItems(Inventory);
        var number = GetNumberOfAction(Inventory.Count, "Выберите какой предмет вы хотите выбросать:");
        var item = Inventory[number - 1];
        Inventory.Remove(item);
        InventoryWeight -= item.Weight;
        Console.WriteLine($"Вы выбросили предмет {item.Name} из инвентаря");
    }
}
