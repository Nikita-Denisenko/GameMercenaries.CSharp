using GameMercenaries.GameLogic.FightLogic;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;

namespace GameMercenaries.UserInterface;

public static class UiHelpers
{
    public static void PrintItems(List<Item> items)
    {
        var currentItemNumber = 1;
        
        foreach (var item in items)
        {
            Console.WriteLine($"{currentItemNumber}. {item.Name} Вес: {item.Weight} кг.");
            currentItemNumber++;
        }    
    }

    public static void PrintWeapons(List<Weapon> weapons)
    {
        var current = 1;
        
        foreach (var weapon in weapons)
        {
            var minDamage = weapon.DamageRange[0];
            var maxDamage = weapon.DamageRange[1];
                
            Console.WriteLine($"{current}. {weapon.Name} Дистанция: {weapon.Distance} Урон: {minDamage}-{maxDamage}");
            current++;
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
    
    public static void PrintLocationGroup(string title, List<Location> locations, Player currentPlayer)
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
    
    public static void PrintPlayerEvents(Player player)
    {
        if (player.History.Count == 0) return;
        
        Console.WriteLine("Внимание!");
        
        foreach (var currentEvent in player.History)
        {
            Console.WriteLine(currentEvent.Message);
        }
        
        Console.WriteLine();
    }

    public static void PrintGameEvents(List<GameEvent> events, bool isCurrentDay)
    {
        Console.WriteLine(isCurrentDay ? "Текущий день" : "Прошлый день");
        
        if (events.Count == 0)
        {
            Console.WriteLine("Список событий пуст");
            return;
        }
        
        foreach (var currentEvent in events)
        {
            Console.WriteLine(currentEvent.Message);
        }
        
        Console.WriteLine();
    }
    
    public static void PrintHandFightResult(HandFightResult handFightResult)
    {
        Console.WriteLine(handFightResult.Message);
        
        if (!handFightResult.WasSuccessful || handFightResult.DefenderDied) return;
        
        Console.WriteLine($"Нанесённый урон: {handFightResult.DamageDealt}");
        Console.WriteLine($"Текущее здоровье игрока: {handFightResult.DefenderCurrentHealth} из {handFightResult.DefenderMaxHealth}");
    }

    public static void PrintGunFightResult(GunFightResult gunFightResult)
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

    public static void PrintHelloText()
    {
        Console.WriteLine("Добро пожаловать в игру Наёмники!");
        Console.WriteLine("Игра представляет собой захватывающий многопользовательский экшен в жанре королевской битвы.");
        Console.WriteLine("Здесь вы находитесь на острове, где вам предстоит сражаться с другими игроками, используя разное оружие,");
        Console.WriteLine("поиграть за мутантов, появившихся из-за влияния радиации, исследовать локации, собирать артефакты и многое другое!");
        Console.WriteLine("Используйте стратегическое мышление, а также всевозможные хитрости, чтобы собрать все три");
        Console.WriteLine("магических артефакта, победить всех конкурентов, и конечно, выжить в этом жутком мире Пост-апокалипсиса!");
        Console.WriteLine("Удачи!");
    }

    public static void PrintGameRules()
    {
        
    }
}