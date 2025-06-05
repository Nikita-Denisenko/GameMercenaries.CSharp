namespace GameMercenaries.GameLogic.FightLogic;

public class FightResult
{
    public bool WasSuccessful { get; init; }
    public int DamageDealt { get; init; }
    public bool DefenderDied { get; init; }
    public int DefenderCurrentHealth { get; init; }
    public int DefenderMaxHealth { get; init; }
    public required string Message { get; init; } 
}