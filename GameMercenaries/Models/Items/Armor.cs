namespace GameMercenaries.models.items;

public class Armor(
    string id,
    string name,
    string itemType,
    int maxQuantity,
    int currentQuantity,
    int weight,
    string info,
    int damageReduction
    ) : Item(id, name, itemType, maxQuantity, currentQuantity, weight, info)
{
    public int DamageReduction { get; } = damageReduction;
}