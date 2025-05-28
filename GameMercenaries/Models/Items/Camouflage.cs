namespace GameMercenaries.models.items;

public class Camouflage(
    string id,
    string name,
    string itemType,
    int weight,
    string info,
    int cutEnemyAccuracyBonus
    ) : Item(id, name, itemType, weight, info)
{
    public int CutEnemyAccuracyBonus { get; } = cutEnemyAccuracyBonus;
}