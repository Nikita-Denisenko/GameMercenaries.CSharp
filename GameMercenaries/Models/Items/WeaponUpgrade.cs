namespace GameMercenaries.models.items;

public class WeaponUpgrade(
    string id,
    string name,
    string itemType,
    int weight,
    string info,
    int accuracyBonus
) : Item(id, name, itemType, weight, info)
{
    public int AccuracyBonus { get; } = accuracyBonus;
}