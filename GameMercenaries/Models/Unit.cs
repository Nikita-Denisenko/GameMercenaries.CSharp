namespace GameMercenaries.models;

public class Unit(
    string id,
    string name,
    int health,
    int actions,
    int weight,
    string info,
    Dictionary<string, int> rules)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public int MaxHealth { get; } = health;
    public int CurrentHealth { get; private set; } = health;
    public int MaxActions { get; } = actions;
    public int CurrentActions { get; private set; } = actions;
    public int Weight { get; } = weight;
    public Dictionary<string, int> Rules { get; } = rules;

    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= Math.Min(CurrentHealth, damage);
    }

    public void RestoreHealth(int healthPoints)
    {
        CurrentHealth = Math.Min(CurrentHealth + healthPoints, MaxHealth);
    }

    public void UseActions(int actions)
    {
        if (CurrentActions < actions)
        {
            Console.WriteLine("У вас недостаточно действий");
            return;
        }
        
        CurrentActions -= actions;
        Console.WriteLine($"Количество потраченых вами действий: {actions}");
    }

    public void RestoreAllActions()
    {
        CurrentActions = MaxActions;
    }
    
    public void PrintInfo()
    {
        Console.WriteLine(info);
        Console.WriteLine($"Имя: {Name}");
        Console.WriteLine($"Здорововье: {CurrentHealth} из {MaxHealth}");
        Console.WriteLine($"Действия: {CurrentActions} из {MaxActions}");
        Console.WriteLine($"Грузоподъёмность: {Weight} кг");
    }
}