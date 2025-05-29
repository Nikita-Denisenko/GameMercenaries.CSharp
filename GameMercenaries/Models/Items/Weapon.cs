namespace GameMercenaries.models.items;

public class Weapon(
    string id,
    string name,
    string itemType,
    int weight,
    string info,
    string weaponType,
    int[] damageRange,
    int distance,
    int accuracy,
    Dictionary<string, int> rules
    
    ) : Item(id, name, itemType, weight, info)
{
    public string WeaponType { get; } = weaponType;
    public int[] DamageRange { get; } = damageRange;
    public int Distance { get; } = distance;
    public int Accuracy { get; } = accuracy;
    public Dictionary<string, int> Rules { get; } = rules;

    public int GenerateDamage()
    {
        Random random = new();
        var first = DamageRange[0];
        var second = DamageRange[1]; 
        return random.Next(first, second + 1);
    }
}