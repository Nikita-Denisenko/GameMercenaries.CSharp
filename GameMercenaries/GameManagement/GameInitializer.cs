using static GameMercenaries.UserInterface.MenuUi;
using static GameMercenaries.UserInterface.UiHelpers;
using static GameMercenaries.GameLogic.EntityLogic.PlayerLogic;

namespace GameMercenaries.gameManagement;


public static class GameInitializer
{
    public static CurrentGame? CreateNewGame()
    {
        var actionsQuantity = PrintCreateGameMenu();

        var playersQuantity = GetNumberOfAction(actionsQuantity, "Введите номер действия:");
        
        if (playersQuantity == 1) return null;

        List<string> playerNames = [];

        for (var i = 1; i <= playersQuantity; i++)
        {
            Console.WriteLine("Чтобы вернуться назад введите любую цифру");
            while (true)
            {
                Console.WriteLine($"Введите имя игрока {i}:");
                var name = Console.ReadLine();
                
                if (string.IsNullOrEmpty(name)) continue;
                
                if (int.TryParse(name, out _))
                {
                    if (i == 1) return null;
                    i-=2;
                    playerNames.RemoveAt(playerNames.Count - 1);
                }
                
                playerNames.Add(name);
                break;
            }
        }

        var players = CreatePlayers(playerNames);
        
        Console.WriteLine("Игра успешно создана!");
        PressEnterToContinue();
        return new CurrentGame(players);
    }
}