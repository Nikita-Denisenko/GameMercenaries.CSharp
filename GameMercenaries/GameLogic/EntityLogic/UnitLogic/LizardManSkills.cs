using GameMercenaries.Constants;
using GameMercenaries.Models;

namespace GameMercenaries.GameLogic.EntityLogic.UnitLogic;

public static class LizardManSkills
{
    public static void LizardManLogic(List<Player> players)
    {
        var lizardPlayer = players
            .FirstOrDefault(player => (UnitIdType)player.Unit.Id == UnitIdType.LizardMan);

        lizardPlayer?.Unit.RestoreHealth((int)lizardPlayer.Unit.Rules["restore_hp_bonus"]);
    }
}