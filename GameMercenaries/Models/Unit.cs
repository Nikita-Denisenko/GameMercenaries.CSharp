using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace GameMercenaries.Models;

public class Unit
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("name")]
    public required string  Name { get; init; }

    [JsonProperty("health")]
    public int MaxHealth { get; init; }

    [JsonProperty("actions")]
    public int MaxActions { get; init;}

    [JsonProperty("weight")]
    public int Weight { get; init; }

    [JsonProperty("info")]
    public required string Info { get; init; }

    [JsonProperty("rules")]
    public Dictionary<string, object> Rules { get; init; } = new();

    [JsonIgnore]
    public int CurrentHealth { get; private set; }

    [JsonIgnore] public int CurrentActions { get; private set; }

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
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