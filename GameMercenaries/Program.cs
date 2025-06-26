using GameMercenaries.gameManagement;
using static GameMercenaries.gameManagement.GameInitializer;
using static GameMercenaries.UserInterface.UiHelpers;
using static GameMercenaries.UserInterface.MenuUi;
    
Main();

void Main()
{
    while (true)
    {
        Console.Clear();
        
        PrintHelloText();
    
        var actionsQuantity = PrintStartMenu();

        var numberOfAction = GetNumberOfAction(actionsQuantity, "Введите номер действия:");
        
        Console.Clear();

        if (numberOfAction == actionsQuantity)
        {
            ShowGameRules();
            PressEnterToContinue();
            continue;
        }
        
        var game = CreateNewGame();
    
        if (game is null)
        {
            Console.Clear();
            continue;
        }
    
        var gameLoop = new GameLoop(game);
    
        gameLoop.Run();

        break;
    }
}
