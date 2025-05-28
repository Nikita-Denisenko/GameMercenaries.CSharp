namespace GameMercenaries.models.items;

public class HealthKit(
    string id,
    string name,
    string itemType,
    int weight,
    string info,
    int hpBonus
    ) : Item(id, name, itemType, weight, info)
{
    public int HpBonus { get; } = hpBonus;
}