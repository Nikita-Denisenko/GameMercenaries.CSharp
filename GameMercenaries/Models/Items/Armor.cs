namespace GameMercenaries.models.items;

public class Armor(
    string id,
    string name,
    string itemType,
    int weight,
    string info,
    int damageReduction
    ) : Item(id, name, itemType, weight, info)
{
    public int DamageReduction { get; } = damageReduction;
}