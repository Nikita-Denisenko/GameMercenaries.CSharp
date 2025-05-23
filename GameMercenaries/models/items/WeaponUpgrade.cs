namespace GameMercenaries.models.items;

public class WeaponUpgrade(
    string id,
    string name,
    string itemType,
    int maxQuantity,
    int currentQuantity,
    int weight,
    string info,
    int accuracyBonus
) : Item(id, name, itemType, maxQuantity, currentQuantity, weight, info)
{
    public int AccuracyBonus { get; } = accuracyBonus;
}