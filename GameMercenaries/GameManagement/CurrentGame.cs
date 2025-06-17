using GameMercenaries.Models;
using GameMercenaries.Models.Items;
using static GameMercenaries.GameLogic.EntityLogic.UnitLogic.LizardManSkills;
using static GameMercenaries.UserInterface.UserInterface;
using static GameMercenaries.GameLogic.FightLogic.FightService;

namespace GameMercenaries.gameManagement;

public class CurrentGame(
    List<Player> players)
{
    private List<Location> Locations { get; } = GameData.Locations;
    public List<Unit> Units { get; } = GameData.Units;
    public List<Item> Items { get; } =  GameData.Items;
    public int DayNumber { get; private set; } = 0;
    public List<Player> AllPlayers { get; } = players;
    public List<Player> AlivePlayers { get; } = players;
    public List<string> GameEvents { get; } = [];

    public void KillPlayer(Player player)
    {
        AlivePlayers.Remove(player);
    }

    public void AddEvent(string eventText)
    {
        GameEvents.Add(eventText);
    }

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
                    var handFightResult = HandFight(attacker, defender);
                    PrintHandFightResult(handFightResult);
                    break;
                case 2:
                    var weapon = attacker.ChooseWeaponForAttack();
                    if (weapon is null) continue;
                    var gunFightResult = GunFight(attacker, defender, weapon);
                    PrintGunFightResult(gunFightResult);
                    break;
            }
        }
    }
    
    public void MapMenuLogic(Player player)
    {
       var quantityActions = MapMenu(player);
       var numberOfAction = GetNumberOfAction(quantityActions, "Введите номер действия:");
       
       if (numberOfAction == quantityActions) return;

       FightMenuLogic(player);
    }
    
    public void InventoryMenuLogic(Player player)
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
                return;
            case "Выбросить предмет":
                player.RemoveItem();
                break;
            case "Использовать аптечку":
                player.UseMedKit();
                break;
        }
    }

    public static void LocationMenuLogic(Player player)
    {
        var actionsQuantity = LocationMenu(player);

        var numberOfAction = GetNumberOfAction(actionsQuantity, "Введите номер действия: ");

        if (actionsQuantity == numberOfAction) return;
        
        player.FindItem();
    }

    public static void UnitMenuLogic(Player player)
    {
        var actionsQuantity = UnitMenu(player);

        var numberOfAction = GetNumberOfAction(actionsQuantity, "Введите номер действия: ");
        
        if (actionsQuantity == numberOfAction) return;
        
        player.UseMedKit();
    }

    public void EventsMenuLogic()
    {
        var actionsQuantity = EventsMenu(GameEvents);
        
        GetNumberOfAction(actionsQuantity, "Введите номер действия: ");
    }

    public void MainMenuLogic()
    {
        
    }
}