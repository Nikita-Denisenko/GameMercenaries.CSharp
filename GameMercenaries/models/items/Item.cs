namespace GameMercenaries.models.items;

public abstract class Item(
    string id,
    string name,
    string itemType,
    int maxQuantity,
    int currentQuantity,
    int weight,
    string info)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string ItemType { get; } = itemType;
    public int MaxQuantity { get; } = maxQuantity;
    public int CurrentQuantity { get; private set; } = currentQuantity;
    public int Weight { get; } = weight;
    public string Info { get; } = info;
}