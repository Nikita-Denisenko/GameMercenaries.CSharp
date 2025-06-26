namespace GameMercenaries.GameLogic.FightLogic;

public abstract class FightResult
{
    public bool PlayersCanFight { get; init; }
    public bool WasSuccessful { get; init; }
    public int DamageDealt { get; init; }
    public bool DefenderDied { get; init; }
    public int DefenderCurrentHealth { get; init; }
    public int DefenderMaxHealth { get; init; }
    public string Message { get; init; } = ""; 
}