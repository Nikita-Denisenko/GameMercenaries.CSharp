using GameMercenaries.Constants;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;
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
    public List<Unit> Units { get; } = GameData.Units;
    public List<Item> Items { get; } =  GameData.Items;
    public int DayNumber { get; private set; }
    public List<Player> AllPlayers { get; } = players;
    public List<Player> AlivePlayers { get; } = players;
    
    public List<GameEvent> History { get; } = [];

    public void KillPlayer(Player player) => AlivePlayers.Remove(player);

    public void AddEvent(GameEvent gameGameEvent) => History.Add(gameGameEvent);

    public void FinishDay()
    {
        LizardManLogic(AlivePlayers);
        DayNumber++;
    }
    
    private Player? ChoosePlayerToAttack(Player attacker)
    {
        while (true)
        {
            var locationsQuantity = ChooseLocationMenu(Locations);
            var actionsQuantity = locationsQuantity + 1;
            
            Console.WriteLine($"{actionsQuantity}. Назад");
            
            var locationNumber = GetNumberOfAction(Locations.Count, "Введите номер локации:");
        
            if (locationNumber == actionsQuantity) return null;
        
            var defenderLocation = Locations[locationNumber - 1];
            
            var availablePlayers = defenderLocation.CurrentPlayers
                .Where(player => player != attacker).ToList();

            var quantityPlayers = availablePlayers.Count;
            
            if (availablePlayers.Count == 0)
            {
                Console.WriteLine("На этой локации нет игроков, выберите другую");
                continue;
            }
            
            ChoosePlayerMenu(availablePlayers);
            
            actionsQuantity = quantityPlayers + 1;
            
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

    private void FightMenuLogic(Player attacker)
    {
        while (true)
        {
            var defender = ChoosePlayerToAttack(attacker);

            if (defender is null) return;

            var fightNumber = ChooseFightType();

            if (fightNumber is null) continue;

            switch (fightNumber)
            {
                case 1:
                    var handFightResult = HandFight(
                        attacker, 
                        defender, 
                        DayNumber,
                        gameEvent => History.Add(gameEvent)
                        );
                    
                    PrintHandFightResult(handFightResult);
                   
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
                    
                    PrintGunFightResult(gunFightResult);
                    
                    break;
            }
        }
    }
    
    private bool MapMenuLogic(Player player)
    {
       var quantityActions = MapMenu(player);
       var numberOfAction = GetNumberOfAction(quantityActions, "Введите номер действия:");

       if (numberOfAction != quantityActions)
       {
           FightMenuLogic(player);
       }
       
       return true;
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

        switch (action)
        {
            case "Назад":
                break;
            case "Выбросить предмет":
                player.RemoveItem();
                break;
            case "Использовать аптечку":
                player.UseMedKit();
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

        var item = player.ItemOnLocation;
        
        player.FindItem();

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
        
        player.UseMedKit();
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
        PrintPlayerEvents(player);
        
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
            
            PrintMoveActionsMenu();

            if ((UnitIdType)player.Unit.Id == UnitIdType.ChameleonMan)
            {
                Console.WriteLine($"{++index}. Украсть предмет у игрока");
                menuOptions[index] = () => ChameleonManLogic(
                    player, 
                    AlivePlayers, 
                    DayNumber, 
                    gameEvent => History.Add(gameEvent)
                    );
            }
            
            var actionsQuantity = menuOptions.Count;

            var numberOfAction = GetNumberOfAction(actionsQuantity, "Введите номер действия:");

            if (!menuOptions[numberOfAction]()) return;
        }
    }
}