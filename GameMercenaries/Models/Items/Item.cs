namespace GameMercenaries.models.items;

public abstract class Item(
    string id,
    string name,
    string itemType,
    int weight,
    string info)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string ItemType { get; } = itemType;
    public int Weight { get; } = weight;
    
    public void PrintInfo() => Console.WriteLine(info);
}