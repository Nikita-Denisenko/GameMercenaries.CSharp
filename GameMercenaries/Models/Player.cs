using GameMercenaries.Constants;
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
    public bool ItemWasTaken { get; private set; }
    public List<Item> Inventory { get; } = [];
    public int InventoryWeight { get; private set; }
    public HashSet<Artefact> Artefacts { get; private set; } = [];
    public List<Event> Events { get; private set; } = [];
    
    public void ChangeLocation()
    {
        Location = GenerateNewLocation(Location);
        ItemOnLocation = GenerateItem(Location);
        ItemWasTaken = false;
    }

    public void AddEvent(Event playerEvent) => Events.Add(playerEvent);
    
    public bool CanTakeItem(Item item) => Unit.Weight >= InventoryWeight + item.Weight;
    
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

    public void UseMedKit()
    {
        var medKit = Inventory
            .OfType<HealthKit>()
            .FirstOrDefault(item => item.Id == (int)ItemIdType.Medkit);

        if (medKit is null)
        {
            Console.WriteLine("В вашем инвентаре нет аптечки!"); 
            return;
        }

        var hpBonus = medKit.HpBonus;
        
        Unit.RestoreHealth(hpBonus);

        Console.WriteLine($"Вы восстановили {hpBonus} здоровья.");
        Console.WriteLine($"Ваше текущее здоровье {Unit.CurrentHealth} из {Unit.MaxHealth}");
    }

    public Weapon? ChooseWeaponForAttack()
    {
        var weapons = Inventory
            .OfType<Weapon>()
            .Where(item => item.Id != (int)ItemIdType.Knife).ToList();
        
        var weaponsQuantity = weapons.Count;

        if (weaponsQuantity == 0)
        {
            Console.WriteLine("У вас в инвентаре нет оружия!");
            return null;
        }
        
        PrintWeapons(weapons);

        var actionsQuantity = weaponsQuantity + 1;
        
        Console.WriteLine($"{actionsQuantity}. Назад");

        var weaponNumber = GetNumberOfAction(actionsQuantity, "Введите номер оружия:");

        if (weaponNumber == actionsQuantity) return null;

        var weapon = weapons[weaponNumber - 1];

        return weapon;
    }
}
