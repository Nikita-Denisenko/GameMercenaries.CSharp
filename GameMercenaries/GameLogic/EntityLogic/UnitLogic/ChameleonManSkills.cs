using GameMercenaries.Models;
using GameMercenaries.Models.Items;
using static GameMercenaries.UserInterface.UserInterface;

namespace GameMercenaries.GameLogic.EntityLogic.UnitLogic;


public static class ChameleonManSkills
{
    private static Player? ChooseTargetPlayer(Player chameleon, List<Player> players)
    {
        var availablePlayers = players
            .Where(player => player.Location == chameleon.Location && player != chameleon).ToList();

        var playersQuantity = availablePlayers.Count;
        
        if (playersQuantity == 0)
        {
            Console.WriteLine("В вашей локации нет игроков, чтобы украсть предмет.");
            return null;
        }

        var actionsQuantity = playersQuantity + 1;
        
        PrintPlayers(availablePlayers, chameleon);
        Console.WriteLine($"{actionsQuantity}. Отмена");

        var playerNumber = GetNumberOfAction(actionsQuantity, "Введите номер игрока, у которого вы хотите украсть предмет");

        if (playerNumber == actionsQuantity) return null;
        
        var enemy = availablePlayers[playerNumber - 1];

        if (enemy.Inventory.Count == 0)
        {
            Console.WriteLine("Инвентарь этого игрока пуст выберите другого!");
            return null;
        }

        return enemy;
    }

    private static Item? ChooseItemToSteal(Player chameleon, List<Item> enemyInventory)
    {
        PrintItems(enemyInventory);
        
        var actionsQuantity = enemyInventory.Count + 1;
        Console.WriteLine($"{actionsQuantity}. Отмена");

        var itemNumber = GetNumberOfAction(actionsQuantity, "Введите номер предмета, который хотите украсть.");

        if (itemNumber == actionsQuantity) return null;
        
        var item = enemyInventory[itemNumber - 1];
        
        if (!chameleon.CanTakeItem(item))
        {
            Console.WriteLine($"В вашем инвентаре недостаточно места, для предмета {item.Name}!");
            return null;
        }

        return item;
    }
    
    public static void ChameleonManLogic(Player chameleon, List<Player> players)
    {
       var enemy = ChooseTargetPlayer(chameleon, players);
       if (enemy is null) return;

       var enemyInventory = enemy.Inventory;
       
       var item = ChooseItemToSteal(chameleon, enemyInventory);

       if (item is null) return;
       
       chameleon.TakeItem(item);
       enemyInventory.Remove(item);
        
       Console.WriteLine($"Вы украли предмет {item.Name} у игрока {enemy.UserName}!");
       Console.WriteLine($"Вес вашего инвентаря: {chameleon.InventoryWeight} кг.");
    }
}