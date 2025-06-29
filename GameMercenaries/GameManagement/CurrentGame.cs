using GameMercenaries.Constants;
using GameMercenaries.Models;
using static GameMercenaries.GameLogic.EntityLogic.UnitLogic.LizardManSkills;
using static GameMercenaries.UserInterface.MenuUi;
using static GameMercenaries.UserInterface.UiHelpers;
using static GameMercenaries.GameLogic.FightLogic.FightService;
using static GameMercenaries.GameLogic.EntityLogic.UnitLogic.ChameleonManSkills;

namespace GameMercenaries.gameManagement;

public class CurrentGame(
    List<Player> players)
{
    private List<Location> Locations { get; } = GameData.Locations;
    private int DayNumber { get; set; } = 1;
    public List<Player> AllPlayers { get; } = players;
    public List<Player> AlivePlayers { get; } = players;
    
    private List<GameEvent> History { get; } = [];

    public bool IsGameOver { get; private set; }
    public Player? Winner { get; private set; } 

    private void KillPlayer(Player player) => AlivePlayers.Remove(player);

    private void AddEvent(GameEvent gameGameEvent) => History.Add(gameGameEvent);

    public void FinishDay()
    {
        LizardManLogic(AlivePlayers);
        
        foreach (var player in AlivePlayers)
        {
            player.Unit.RestoreAllActions();
        }
        
        DayNumber++;
    }

    private Location? ChooseLocationToAttackPlayer()
    {
        var locationsQuantity = ChooseLocationMenu(Locations);
        var actionsQuantity = locationsQuantity + 1;
            
        Console.WriteLine($"{actionsQuantity}. Назад");
            
        var locationNumber = GetNumberOfAction(actionsQuantity, "Введите номер локации:");
        
        if (locationNumber == actionsQuantity) return null;
        
        var defenderLocation = Locations[locationNumber - 1];

        return defenderLocation;
    }
    
    private Player? ChoosePlayerToAttack(Player attacker, Location defenderLocation)
    {
        while (true)
        {
            var availablePlayers = defenderLocation.CurrentPlayers
                .Where(player => player != attacker).ToList();

            var quantityPlayers = availablePlayers.Count;
            
            if (availablePlayers.Count == 0)
            {
                Console.WriteLine("На этой локации нет игроков, выберите другую");
                PressEnterToContinue();
                return null;
            }
            
            ChoosePlayerMenu(availablePlayers);
            
            var actionsQuantity = quantityPlayers + 1;
            
            Console.WriteLine($"{actionsQuantity}. Назад");
            
            var defenderNumber = GetNumberOfAction(actionsQuantity, "Введите номер игрока: ");
            if (defenderNumber == actionsQuantity) return null;
            
            var defender = availablePlayers[defenderNumber - 1];

            return defender;
        }
    }

    private static int? ChooseFightType()
    {
        var fightTypesQuantity = ChooseFightTypeMenu();

        var actionsQuantity = fightTypesQuantity + 1;
        
        Console.WriteLine($"{actionsQuantity}. Назад");

        var fightNumber = GetNumberOfAction(actionsQuantity, "Введите номер действия:");

        if (fightNumber == actionsQuantity) return null;

        return fightNumber;
    }

    private bool FightMenuLogic(Player attacker)
    {
        const int actionCost = 1;
        
        while (true)
        {
            Console.Clear();

            var defenderLocation = ChooseLocationToAttackPlayer();

            if (defenderLocation is null) return false;
            
            Console.Clear();
            
            var defender = ChoosePlayerToAttack(attacker, defenderLocation);

            if (defender is null) continue;
            
            Console.Clear();

            var fightNumber = ChooseFightType();

            if (fightNumber is null) continue;
            
            Console.Clear();

            switch (fightNumber)
            {
                case 1:
                    
                    var handFightResult = HandFight(
                        attacker, 
                        defender, 
                        DayNumber,
                        gameEvent => History.Add(gameEvent)
                        );
                    
                    if (handFightResult.PlayersCanFight) attacker.Unit.UseActions(actionCost);
                    
                    if (handFightResult.DefenderDied) KillPlayer(defender);
                    
                    PrintHandFightResult(handFightResult);
                    PressEnterToContinue();
                   
                    break;
               
                case 2:
                    var weapon = attacker.ChooseWeaponForAttack();
                    
                    if (weapon is null) continue;
                   
                    var gunFightResult = GunFight(
                        attacker, 
                        defender, 
                        weapon, 
                        DayNumber,
                        gameEvent => History.Add(gameEvent));
                    
                    if (gunFightResult.PlayersCanFight) attacker.Unit.UseActions(actionCost);
                    
                    if (gunFightResult.DefenderDied) KillPlayer(defender);
                    
                    PrintGunFightResult(gunFightResult);
                    PressEnterToContinue();
                    
                    break;
            }

            if (AlivePlayers.Count == 1)
            {
                IsGameOver = true;
                Winner = attacker;
            }
            
            return true;
        }
    }
    
    private bool MapMenuLogic(Player player)
    {
        while (true)
        {
            Console.Clear();
            var quantityActions = MapMenu(player);
            var numberOfAction = GetNumberOfAction(quantityActions, "Введите номер действия:");

            if (numberOfAction == quantityActions)
            {
                return true;
            }
       
            const int actionCost = 1;
       
            Console.Clear();
        
            if (player.Unit.CurrentActions < actionCost)
            {
                Console.WriteLine("У вас недостаточно действий. Дождитесь следующего хода.");
                PressEnterToContinue();
                continue;
            }
       
            if (!FightMenuLogic(player)) continue; 
       
            return true;
        }
    }
    
    private bool InventoryMenuLogic(Player player)
    {
        var options = GetInventoryMenuOptions(player.Inventory)
            .OrderBy(pair => pair.Key).ToDictionary();

        foreach (var (key, value) in options)
        {
            Console.WriteLine($"{key}. {value}");
        }

        var numberOfAction = GetNumberOfAction(options.Count, "Введите номер действия: ");

        var action = options[numberOfAction];
        
        Console.Clear();

        switch (action)
        {
            case "Назад":
                break;
            
            case "Выбросить предмет":
                player.RemoveItem();
                PressEnterToContinue();
                break;
           
            case "Использовать аптечку (1 действие)":
                const int actionCost = 1;
                
                if (player.Unit.CurrentActions < actionCost)
                {
                    Console.WriteLine("У вас недостаточно действий. Дождитесь следующего хода");
                    PressEnterToContinue();
                    return true;
                }
                
                player.Unit.UseActions(actionCost);
                player.UseMedKit();
                PressEnterToContinue();
                
                var unit = player.Unit;
                
                var newGameEvent = new GameEvent
                {
                    Day = DayNumber,
                    Type = EventType.Heal,
                    Message = $"Игрок {player.UserName} использовал аптечку! Его здоровье {unit.CurrentHealth} из {unit.MaxHealth}"
                };
                
                AddEvent(newGameEvent);
                
                break;
        }

        return true;
    }

    private bool LocationMenuLogic(Player player)
    {
        var actionsQuantity = LocationMenu(player);

        var numberOfAction = GetNumberOfAction(actionsQuantity, "Введите номер действия: ");

        if (actionsQuantity == numberOfAction) return true;

        const int actionCost = 1;
        
        Console.Clear();

        if (player.Unit.CurrentActions < actionCost)
        {
            Console.WriteLine("У вас недостаточно действий. Дождитесь следующего хода.");
            PressEnterToContinue();
            return true;
        }
        
        var item = player.ItemOnLocation;
        
        player.Unit.UseActions(actionCost);
        player.FindItem();
        PressEnterToContinue();

        GameEvent newGameEvent;
        
        if (item.ItemType == "Артефакт")
        {
            newGameEvent = new GameEvent
            {
                Day = DayNumber,
                Type = EventType.ArtefactFound,
                Message = $"Игрок {player.UserName} взял Артефакт {item.Name}!"
            };
            
            AddEvent(newGameEvent);

            if (player.HasAllArtefacts())
            {
                IsGameOver = true;
                Winner = player;
            }
        
            return true;
        }
        
        newGameEvent = new GameEvent
        {
            Day = DayNumber,
            Type = EventType.ItemFound,
            Message = $"Игрок {player.UserName} взял предмет {item.Name}"
        };
        
        AddEvent(newGameEvent);
        
        return true;
    }

    private bool UnitMenuLogic(Player player)
    {
        var actionsQuantity = UnitMenu(player);

        var numberOfAction = GetNumberOfAction(actionsQuantity, "Введите номер действия: ");
        
        if (actionsQuantity == numberOfAction) return true;

        const int actionCost = 1;
        
        if (player.Unit.CurrentActions < actionCost)
        {
            Console.WriteLine("У вас недостаточно действий. Дождитесь следующего хода");
            PressEnterToContinue();
            return true;
        }
                
        player.Unit.UseActions(actionCost);
        player.UseMedKit();
        PressEnterToContinue();
        
        var unit = player.Unit;
        
        var newGameEvent = new GameEvent
        {
            Day = DayNumber,
            Type = EventType.Heal,
            Message = $"Игрок {player.UserName} использовал аптечку! Его здоровье {unit.CurrentHealth} из {unit.MaxHealth}"
        };
        AddEvent(newGameEvent);
        return true;
    }

    private bool EventsMenuLogic()
    {
        var actionsQuantity = EventsMenu(History, DayNumber);
        
        GetNumberOfAction(actionsQuantity, "Введите номер действия: ");

        return true;
    }

    private bool FinishDayForPlayer(Player player)
    {
        player.ChangeLocation();

        var newGameEvent = new GameEvent
        {
            Day = DayNumber,
            Type = EventType.LocationChanged,
            Message = $"Игрок {player.UserName} переместился в локацию {player.Location.Name}"
        };

        var newPlayerGameEvent = new GameEvent
        {
            Day = DayNumber,
            Type = EventType.LocationChanged,
            Message = $"Вы переместились в локацию {player.Location.Name}"
        };
        
        AddEvent(newGameEvent);
        player.AddEvent(newPlayerGameEvent);
        
        return false;
    }

    public void MainMenuLogic(Player player)
    {
        var firstThisDayEntry = true;
        
        while (true)
        {
            var index = 0;
            
            var menuOptions = new Dictionary<int, Func<bool>>()
            {
                [++index] = () => MapMenuLogic(player),
                [++index] = () => InventoryMenuLogic(player),
                [++index] = () => LocationMenuLogic(player),
                [++index] = () => UnitMenuLogic(player),
                [++index] = EventsMenuLogic,
                [++index] = () => FinishDayForPlayer(player)
            };

            var playerUnit = player.Unit;
            
            Console.Clear();
            Console.WriteLine($"Ходит игрок {player.UserName}");
            Console.WriteLine($"День {DayNumber}");
            if (firstThisDayEntry)
            {
                PrintPlayerEvents(player);
            } 
            player.ClearHistory();
            Console.WriteLine($"Ваш юнит: {playerUnit.Name}");
            Console.WriteLine($"Жизни {playerUnit.CurrentHealth} из {playerUnit.MaxHealth}");
            Console.WriteLine($"Количество действий: {playerUnit.CurrentActions} из {playerUnit.MaxActions}");
            Console.WriteLine($"Загруженность инвентаря: {player.InventoryWeight} из {playerUnit.Weight} кг");
            PrintMoveActionsMenu();

            if ((UnitIdType)playerUnit.Id == UnitIdType.ChameleonMan)
            {
                Console.WriteLine($"{++index}. Украсть предмет у игрока (1 действие)");
                menuOptions[index] = () => ChameleonManLogic(
                    player, 
                    AlivePlayers, 
                    DayNumber, 
                    gameEvent => History.Add(gameEvent)
                    );
            }
            
            var actionsQuantity = menuOptions.Count;

            var numberOfAction = GetNumberOfAction(actionsQuantity, "Введите номер действия:");

            firstThisDayEntry = false;

            Console.Clear();

            if (!menuOptions[numberOfAction]())
            {
                return;
            }
        }
    }
}