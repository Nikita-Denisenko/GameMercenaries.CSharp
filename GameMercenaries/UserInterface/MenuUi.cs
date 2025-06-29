using GameMercenaries.Constants;
using GameMercenaries.gameManagement;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;
using static GameMercenaries.UserInterface.UiHelpers;

namespace GameMercenaries.UserInterface;

public static class MenuUi
{
    public static int ChooseLocationMenu(List<Location> locations)
    {
        var current = 1;
        
        foreach (var location in locations)
        {
            Console.WriteLine($"{current}. {location.Name}");
            current++;
        }
        
        return locations.Count;
    }

    public static void ChoosePlayerMenu(List<Player> availablePlayers)
    {
        var current = 1;
        
        foreach (var player in availablePlayers)
        {
            Console.WriteLine($"{current}. {player.UserName}");
            current++;
        }
    }

    public static int ChooseFightTypeMenu()
    {
        const int actionsQuantity = 2;

        Console.WriteLine("1. Атаковать врукопашную");
        Console.WriteLine("2. Стрелять из оружия");

        return actionsQuantity;
    }
    
    public static void PrintMoveActionsMenu()
    {
        Console.WriteLine("1. Карта");
        Console.WriteLine("2. Инвентарь");
        Console.WriteLine("3. Осмотреть локацию");
        Console.WriteLine("4. Мой юнит");
        Console.WriteLine("5. События");
        Console.WriteLine("6. Покинуть локацию и завершить день.");
    }
    
    public static int MapMenu(Player currentPlayer)
    {
        const int actionsQuantity = 2;
        
        var playerLocation = currentPlayer.Location;
        var adjacentLocations = playerLocation.AdjacentLocations
            .Select(id => GameData.Locations[id - 1]).ToList();
        var distantLocations = playerLocation.DistantLocations
            .Select(id => GameData.Locations[id - 1]).ToList();
        var unavailableLocations = playerLocation.UnavailableLocations
            .Select(id => GameData.Locations[id - 1]).ToList();
        
        Console.WriteLine($"Ваша локация ({playerLocation.Name}):");
        PrintPlayers(playerLocation.CurrentPlayers, currentPlayer);
        Console.WriteLine();

        PrintLocationGroup("Соседние локации", adjacentLocations, currentPlayer);
        
        PrintLocationGroup("Дальние", distantLocations, currentPlayer);
        
        PrintLocationGroup("Недоступные", unavailableLocations, currentPlayer);
        
        Console.WriteLine("1. Атаковать игрока (1 действие)");
        Console.WriteLine("2. Назад");

        return actionsQuantity;
    }
    
    public static Dictionary<int, string> GetInventoryMenuOptions(List<Item> inventory)
    {
        var options = new Dictionary<int, string>();
        var optionNumber = 0;

        if (inventory.Count == 0)
        {
            Console.WriteLine("В вашем инвентаре нет предметов.");
            Console.WriteLine();
            options[++optionNumber] = "Назад";
            return options;
        }
        
        options[++optionNumber] = "Назад";

        Console.WriteLine("Ваши предметы:");
        PrintItems(inventory);
        Console.WriteLine();

        options[++optionNumber] = "Выбросить предмет";

        var hasMedkit = inventory.Any(item => (ItemIdType)item.Id == ItemIdType.Medkit);
        if (hasMedkit)
        {
            options[++optionNumber] = "Использовать аптечку (1 действие)";
        }

        return options;
    }

    
    public static int LocationMenu(Player player)
    {
        const int actionsQuantity = 1;
        
        player.Location.PrintInfo();
        Console.WriteLine();
        
        if (player.ItemWasTaken)
        {
            Console.WriteLine("1. Назад");
            return actionsQuantity;
        }
        
        var itemName = player.ItemOnLocation.Name;
        
        Console.WriteLine($"В локации есть предмет {itemName}!");
        Console.WriteLine($"1. Взять предмет {itemName} (1 действие)");
        Console.WriteLine("2. Назад");
        
        return actionsQuantity + 1;
    }

    public static int UnitMenu(Player player)
    {
        const int actionsQuantity = 2;
        
        player.Unit.PrintInfo();
        Console.WriteLine();
        
        Console.WriteLine("1. Восстановить здоровье (1 действие)");
        Console.WriteLine("2. Назад");

        return actionsQuantity;
    }
    
    public static int EventsMenu(List<GameEvent> events, int dayNumber)
    {
        const int actionsQuantity = 1;

        var currentDayEvents = events
            .Where(gameEvent => gameEvent.Day == dayNumber).ToList();
        
        var lastDayEvents = events
            .Where(gameEvent => gameEvent.Day == dayNumber - 1).ToList();
        
        PrintGameEvents(currentDayEvents, true);
        PrintGameEvents(lastDayEvents, false);
        
        Console.WriteLine("1. Назад");
        return actionsQuantity;
    }

    public static int PrintPistolsMenu(int quantityPistols)
    {
        const int actionsQuantity = 2;
        
        Console.WriteLine($"Количество пистолетов в вашем инветаре: {quantityPistols}.");
        Console.WriteLine("1. Стрелять из одного");
        Console.WriteLine("2. Стрелять из двух");

        return  actionsQuantity;
    }

    public static int PrintCreateGameMenu()
    {
        const int actionsQuantity = 6;
        
        Console.WriteLine("Выберите количество игроков:");
        Console.WriteLine("1. Назад");
        Console.WriteLine("2. Два игрока");
        Console.WriteLine("3. Три игрока");
        Console.WriteLine("4. Четыре игрока");
        Console.WriteLine("5. Пять игроков");
        Console.WriteLine("6. Шесть игроков");

        return actionsQuantity;
    }
    
    public static int PrintStartMenu()
    {
        const int actionsQuantity = 2;

        Console.WriteLine("1. Создать игру");
        Console.WriteLine("2. Ознакомиться с правилами");

        return actionsQuantity;
    }
}