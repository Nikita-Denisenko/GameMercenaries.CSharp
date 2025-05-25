namespace GameMercenaries.models.items;

public class Camouflage(
    string id,
    string name,
    string itemType,
    int maxQuantity,
    int currentQuantity,
    int weight,
    string info,
    int cutEnemyAccuracyBonus
    ) : Item(id, name, itemType, maxQuantity, currentQuantity, weight, info)
{
    public int CutEnemyAccuracyBonus { get; } = cutEnemyAccuracyBonus;
}