namespace GameMercenaries.gameManagement;

public class GameLoop(CurrentGame game)
{
    public void Run()
    {
        while (true)
        {
            foreach (var player in game.AlivePlayers)
            {
                Console.Clear();
                game.MainMenuLogic(player);
                
                if (game.Winner is null) continue;
                
                Console.Clear();
                Console.WriteLine("Игра завершена!");
                Console.WriteLine($"Победитель игрок {game.Winner.UserName}");
                return;
            }

            game.FinishDay();
        }
    }
}