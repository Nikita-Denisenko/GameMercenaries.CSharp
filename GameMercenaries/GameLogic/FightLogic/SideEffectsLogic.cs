using GameMercenaries.Constants;
using GameMercenaries.gameManagement;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;
using static GameMercenaries.GameLogic.FightLogic.FightHelpers;
using static GameMercenaries.UserInterface.UserInterface;

namespace GameMercenaries.GameLogic.FightLogic;

public static class SideEffectsLogic
{
      private static readonly Random Random = new();
      public static void InjuryLogic(
        Player attacker, 
        bool attackWasSuccessful,
        out bool gotInjury,
        out int damageOfInjury,
        out bool attackerDied
        )
    {
        const LocationIdType chemicalPlant = LocationIdType.ChemicalPlant;

        if (chemicalPlant != (LocationIdType)attacker.Location.Id)
        {
            gotInjury = false;
            damageOfInjury = 0;
            attackerDied = false;
            return;
        }
        
        const int minPercent = 1;
        const int maxPercent = 100;
        
        // Если игрок попал в цель, вероятность получить травму 10%, если нет 50%.
        var injuryProbability = attackWasSuccessful ? 10 : 50;
        var number = Random.Next(minPercent, maxPercent + 1);

        gotInjury = number <= injuryProbability;
        if (!gotInjury)
        {
            damageOfInjury = 0;
            attackerDied = false;
            return;
        }
        
        const int minDamage = 10;
        const int maxDamage = 30;
        
        damageOfInjury = Random.Next(minDamage, maxDamage + 1);
        
        attacker.Unit.TakeDamage( damageOfInjury);

        attackerDied = !attacker.Unit.IsAlive();
    }

    public static void GrenadeLauncherLogic(
        Player attacker, 
        Player defender, 
        Weapon weapon,
        out List<Player> hitByRpgPlayers,
        out List<string> messagesForHitPlayers
        )
    {
        const ItemIdType rpg = ItemIdType.Rpg32;
        
        if ((ItemIdType)weapon.Id != rpg)
        {
            hitByRpgPlayers = [];
            messagesForHitPlayers = [];
            return;
        }

        List<Player> excludedPlayers = [attacker, defender];

        var playersInCurrentLocation = defender.Location.CurrentPlayers
            .Where(player => !excludedPlayers.Contains(player)).ToList();

        List<Player> adjacentPlayers = [];

        foreach (var number in  defender.Location.AdjacentLocations)
        {
            var location = GameData.Locations[number - 1];
            var players = location.CurrentPlayers
                .Where(player => !excludedPlayers.Contains(player)).ToList();
            adjacentPlayers.AddRange(players);
        }

        hitByRpgPlayers = adjacentPlayers.Concat(playersInCurrentLocation).ToList();

        messagesForHitPlayers = [];
        
        const int minDamage = 10;
        const int maxDamage = 25;
        
        foreach (var player in hitByRpgPlayers)
        {
            var damage = Random.Next(minDamage, maxDamage + 1);
            
            player.Unit.TakeDamage(damage);
            
            var message = $"Игрок {player.UserName} получил {damage} урона от осколочного ранения."
                + (player.Unit.IsAlive() 
                    ? "" 
                    : " Игрок умер от последствий.");
            
            messagesForHitPlayers.Add(message);
        }
    }

    public static void TwoPistolsLogic(
        Player attacker, 
        Weapon weapon, 
        int accuracy,
        out bool weaponIsPistol,
        out int totalDamage
        )
    {
        const ItemIdType pistol = ItemIdType.PistolP350;
        
        if ((ItemIdType)weapon.Id != pistol)
        {
            totalDamage = 0;
            weaponIsPistol = false;
            return;
        }

        var resultDamage = 0;
        var quantityPistols = attacker.Inventory.Count(item => (ItemIdType)item.Id == pistol);
        var shots = 1;
        
        if (quantityPistols >= 2)
        {
            var actionsQuantity = PrintPistolsMenu(quantityPistols);
            shots = GetNumberOfAction(actionsQuantity, "Введите номер действия:");
        }

        var changedTwoPistols = shots == 2;
        
        for(var i = 1; i <= shots; i++)
        {
            resultDamage += AttackWasSuccessful(changedTwoPistols ? accuracy - 1 : accuracy)
                ? weapon.GenerateDamage()
                : 0;
        }

        totalDamage = resultDamage;
        weaponIsPistol = true;
    }

    private static bool EquipmentIsBroken(int breakEquipmentChance)
    {
        const int minPercent = 1;
        const int maxPercent = 100;
        
        var number = Random.Next(minPercent, maxPercent + 1);

        return number <= breakEquipmentChance;
    }

    private static bool TryBreakEquipmentById(Player defender, ItemIdType itemId)
    {
        var item = defender.Inventory.FirstOrDefault(item => item.Id == (int)itemId);

        if (item is null) return false;
        
        switch ((ItemIdType)item.Id)
        {
            case ItemIdType.BodyArmor:
                var bodyArmor = (Armor)item;
                if (EquipmentIsBroken(bodyArmor.BreakChance))
                {
                    defender.Inventory.Remove(bodyArmor);
                    return true;
                }
                break;
            
            case ItemIdType.CamouflageCloak:
                var camouflageCloak = (Camouflage)item;
                if (EquipmentIsBroken(camouflageCloak.BreakChance))
                {
                    defender.Inventory.Remove(camouflageCloak);
                    return true;
                }
                break;
        }
        
        return false;
    }

    public static List<string> BreakEquipmentLogic(Player defender)
    {
        const ItemIdType bodyArmor = ItemIdType.BodyArmor;
        const ItemIdType camouflageCloak = ItemIdType.CamouflageCloak;

        List<string> messages = [];
        
        if (TryBreakEquipmentById(defender, bodyArmor)) 
            messages.Add($"Бронежилет игрока {defender.UserName} разбит от вашей атаки!");
        
        if (TryBreakEquipmentById(defender, camouflageCloak))
            messages.Add($"Маскировочный плащ игрока {defender.UserName} разбит от вашей атаки!");

        return messages;
    }
}
