using GameMercenaries.Models;

namespace GameMercenaries.GameLogic.FightLogic;

public class GunFightResult : FightResult
{
    public List<Player> HitByRpgPlayers { get; init; } = [];
    public List<string> MessagesForHitPlayers { get; init; } = [];
    public bool AttackerGotInjuredAtFactory { get; init; }
    public int DamageOfInjury { get; init; }
    public bool AttackerDiedByInjury { get; init; }
    public string? SideEffectMessage { get; init; }
    public List<string> EquipmentBreakMessages { get; init; } = [];
}