using GameMercenaries.Constants;
using GameMercenaries.GameLogic.FightLogic;
using GameMercenaries.gameManagement;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;

namespace GameMercenaries.UserInterface;

public static class UserInterface
{
    public static void PrintItems(List<Item> items)
    {
        var currentItemNumber = 0;
        
        foreach (var item in items)
        {
            currentItemNumber++;
            Console.WriteLine($"{currentItemNumber}. {item.Name} Вес: {item.Weight} кг.");
        }    
    }

    public static void PrintPlayers(List<Player> players, Player currentPlayer)
    {
        var currentPlayerNumber = 0;
        
        if (players.Contains(currentPlayer))
        {
            Console.WriteLine($"-> {currentPlayer.UserName} (Вы) <-");
        }
        
        foreach (var player in players.Where(player => player != currentPlayer))
        {
            currentPlayerNumber++;
            var unit = player.Unit;
            Console.WriteLine($"{currentPlayerNumber}. {player.UserName} ({unit.Name}) Жизни: {unit.CurrentHealth} из {unit.MaxHealth}");
        }    
    }

    private static void PrintLocationGroup(string title, List<Location> locations, Player currentPlayer)
    {
        Console.WriteLine(title);
        foreach (var location in locations)
        {
            Console.WriteLine($"Локация {location.Name}:");
            PrintPlayers(location.CurrentPlayers, currentPlayer);
        }
        Console.WriteLine();
    }
    
    public static int GetNumberOfAction(int actionsQuantity, string message)
    {
        while (true)
        {
            Console.WriteLine(message);
                
            var input = Console.ReadLine();
            
            if (String.IsNullOrEmpty(input) || !int.TryParse(input, out var number))
            {
                Console.WriteLine("Некорректный ввод! попробуйте ещё раз.");
                Console.WriteLine("Введите номер:");
                continue;
            }

            if (number < 1 || number > actionsQuantity)
            {
                Console.WriteLine("Некорректный ввод! попробуйте ещё раз.");
                Console.WriteLine("Введите номер:");
                continue;
            }

            return number;
        }
    }
    
    public static int MoveActionsMenu()
    {
        const int actionsQuantity = 6;
        
        Console.WriteLine("1. Карта");
        Console.WriteLine("2. Инвентарь");
        Console.WriteLine("3. Осмотреть локацию");
        Console.WriteLine("4. Мой юнит");
        Console.WriteLine("5. События");
        Console.WriteLine("6. Покинуть локацию и завершить день.");

        return actionsQuantity;
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
        
        Console.WriteLine("1. Атаковать игрока");
        Console.WriteLine("2. Назад");

        return actionsQuantity;
    }
    
    public static int InventoryMenu(List<Item> inventory)
    {
        const int actionsQuantity = 1;
        
        if (inventory.Count == 0)
        {
            Console.WriteLine("В вашем инвентаре нет предметов.");
            Console.WriteLine();
            Console.WriteLine("1. Назад");
            
            return actionsQuantity;
        }
        
        Console.WriteLine("Ваши предметы:");
        PrintItems(inventory);
        Console.WriteLine();
        
        Console.WriteLine("1. Выбросить предмет");
        
        var hasMedkit = inventory.Any(item => (ItemIdType)item.Id == ItemIdType.Medkit);
        
        if (!hasMedkit)
        {
            Console.WriteLine("2. Назад");
            return actionsQuantity + 1;
        }
        Console.WriteLine("2. Использовать аптечку");
        Console.WriteLine("3. Назад");
        
        return actionsQuantity + 2;
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
        Console.WriteLine($"1. Взять предмет {itemName}");
        Console.WriteLine("2. Назад");
        
        return actionsQuantity + 1;
    }

    public static int UnitMenu(Unit unit)
    {
        const int actionsQuantity = 2;
        
        unit.PrintInfo();
        Console.WriteLine();
        
        Console.WriteLine("1. Восстановить здоровье");
        Console.WriteLine("2. Назад");

        return actionsQuantity;
    }
    
    public static int EventsMenu(List<string> gameEvents)
    {
        const int actionsQuantity = 1;
        
        if (gameEvents.Count == 0)
        {
            Console.WriteLine("Список событий пуст");
            Console.WriteLine("1. Назад");
            return actionsQuantity;
        }
        
        for (var i = gameEvents.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(gameEvents[i]);
        }
        
        Console.WriteLine();
        
        Console.WriteLine("1. Назад");
        return actionsQuantity;
    }

    public static void PrintHandFightResult(HandFightResult handFightResult)
    {
        Console.WriteLine(handFightResult.Message);
        
        if (!handFightResult.WasSuccessful || handFightResult.DefenderDied) return;
        
        Console.WriteLine($"Нанесённый урон: {handFightResult.DamageDealt}");
        Console.WriteLine($"Текущее здоровье игрока: {handFightResult.DefenderCurrentHealth} из {handFightResult.DefenderMaxHealth}");
    }

    public static void PrintGunFight(GunFightResult gunFightResult)
    {
        Console.WriteLine(gunFightResult.Message); 

        if (!gunFightResult.WasSuccessful)
        {
            if (!string.IsNullOrEmpty(gunFightResult.SideEffectMessage))
            {
                Console.WriteLine(gunFightResult.SideEffectMessage);
            }
            return; 
        }

        foreach (var message in gunFightResult.EquipmentBreakMessages)
        {
            Console.WriteLine(message);
        }

        if (!gunFightResult.DefenderDied)
        {
            Console.WriteLine($"Нанесённый урон: {gunFightResult.DamageDealt}");
            Console.WriteLine($"Текущее здоровье: {gunFightResult.DefenderCurrentHealth} из {gunFightResult.DefenderMaxHealth}");
        }

        foreach (var message in gunFightResult.MessagesForHitPlayers) 
        {
            Console.WriteLine(message);
        }

        if (!string.IsNullOrEmpty(gunFightResult.SideEffectMessage)) 
        {
            Console.WriteLine(gunFightResult.SideEffectMessage); 
        }
    }

    public static int PrintPistolsMenu(int quantityPistols)
    {
        const int actionsQuantity = 2;
        
        Console.WriteLine($"Количество пистолетов в вашем инветаре: {quantityPistols}.");
        Console.WriteLine("1. Стрелять из одного");
        Console.WriteLine("2. Стрелять из двух");

        return  actionsQuantity;
    }
}