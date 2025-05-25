namespace GameMercenaries.models.items;

public class HealthKit(
    string id,
    string name,
    string itemType,
    int maxQuantity,
    int currentQuantity,
    int weight,
    string info,
    int hpBonus
    ) : Item(id, name, itemType, maxQuantity, currentQuantity, weight, info)
{
    public int HpBonus { get; } = hpBonus;
}