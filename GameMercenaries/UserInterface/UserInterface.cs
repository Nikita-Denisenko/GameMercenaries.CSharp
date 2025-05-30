using GameMercenaries.models;
using GameMercenaries.models.items;

namespace GameMercenaries.UserInterface;

public static class UserInterface
{
    public static void PrintItems(List<Item> items)
    {
        var currentItemNumber = 0;
        
        foreach (var item in items)
        {
            currentItemNumber++;
            Console.WriteLine($"{currentItemNumber}. {item.Name}");
        }    
    }

    public static void PrintPlayers(List<Player> players)
    {
        var currentPlayerNumber = 0;
        
        foreach (var player in players)
        {
            currentPlayerNumber++;
            var unit = player.Unit;
            Console.WriteLine($"{currentPlayerNumber}. {player.UserName} ({unit.Name}) Жизни: {unit.CurrentHealth} из {unit.MaxHealth}");
        }    
    }
    
    public static int GetNumberOfAction(int actionsQuantity)
    {
        while (true)
        {
            // Номер чего(действия, игрока, предмета) вводить, логичнее записать в вышестоящий
            // метод, так как это зависит от контекста.
                
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
    
    public static void MoveActionsMenu()
    {
        Console.WriteLine("1. Карта");
        Console.WriteLine("2. Инвентарь");
        Console.WriteLine("3. Осмотреть локацию");
        Console.WriteLine("4. Мой юнит");
        Console.WriteLine("5. События");
    }
    
    public static void MapMenu(Player player)
    { 
        return;
    }
    
    public static void InventoryMenu(Player player)
    { 
        return;
    }
    
    public static void LocationMenu(Player player)
    { 
        return;
    }

    public static void UnitMenu(Player player)
    {
        return;
    }
    
    public static void EventsMenu(Player player)
    {
        return;
    }
}