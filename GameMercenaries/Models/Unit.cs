using Newtonsoft.Json;

namespace GameMercenaries.models;

public class Unit
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int MaxHealth { get; init; }
    public int MaxActions { get; init; }
    public int Weight { get; init; }
    public string Info { get; init; }
    public Dictionary<string, int> Rules { get; init; } = [];

    [JsonIgnore]
    public int CurrentHealth { get; private set; }

    [JsonIgnore]
    public int CurrentActions { get; private set; }

    [JsonConstructor]
    public Unit()
    {
        CurrentHealth = MaxHealth;
        CurrentActions = MaxActions;
    }

    public bool IsAlive() => CurrentHealth > 0;

    public void TakeDamage(int damage)
    {
        CurrentHealth -= Math.Min(CurrentHealth, damage);
    }

    public void RestoreHealth(int hp)
    {
        CurrentHealth = Math.Min(CurrentHealth + hp, MaxHealth);
    }

    public void UseActions(int actions)
    {
        if (CurrentActions < actions)
        {
            Console.WriteLine("У вас недостаточно действий");
            return;
        }

        CurrentActions -= actions;
        Console.WriteLine($"Количество потраченных вами действий: {actions}");
    }

    public void RestoreAllActions() => CurrentActions = MaxActions;

    public void PrintInfo()
    {
        Console.WriteLine(Info);
        Console.WriteLine($"Имя: {Name}");
        Console.WriteLine($"Здоровье: {CurrentHealth} из {MaxHealth}");
        Console.WriteLine($"Действия: {CurrentActions} из {MaxActions}");
        Console.WriteLine($"Грузоподъёмность: {Weight} кг");
    }
}