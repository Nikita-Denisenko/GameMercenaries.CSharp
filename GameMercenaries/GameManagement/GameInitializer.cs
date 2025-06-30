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
        
        Console.Clear();

        for (var i = 1; i <= playersQuantity; i++)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Чтобы вернуться назад введите любую цифру");
                Console.WriteLine($"Введите имя игрока {i}:");
                var name = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(name)) continue;

                name = name.Trim();
                
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
        
        Console.Clear();

        var players = CreatePlayers(playerNames);
        
        Console.WriteLine("Игра успешно создана!");
        PressEnterToContinue();
        return new CurrentGame(players);
    }
}