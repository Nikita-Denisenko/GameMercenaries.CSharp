using GameMercenaries.Constants;
using GameMercenaries.gameManagement;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;

namespace GameMercenaries.GameLogic.FightLogic;

public static class FightHelpers
{
    public static int CalculateHandDamage(Unit attackerUnit, List<Item> inventory)
    {
        const int minDamage = 15;
        const int maxDamage = 30;
        const string handAttackBonus = "hand_attack_bonus";
        
        var baseDamage = new Random().Next(minDamage, maxDamage + 1);
        var unitId = (UnitIdType)attackerUnit.Id;

        if (unitId == UnitIdType.CrabMan) 
            return baseDamage + (int)attackerUnit.Rules[handAttackBonus];

        var damage = baseDamage;

        var knife = (Weapon)GameData.Items[(int)ItemIdType.Knife - 1];

        if (inventory.Any(item => item.Id == knife.Id)) 
            damage = knife.GenerateDamage();
        
        if (unitId == UnitIdType.Brawler) 
            damage += (int)attackerUnit.Rules[handAttackBonus];

        return damage;
    }
    
    public static int CalculateWeaponDamage(Weapon attackerWeapon, Unit defenderUnit, List<Item> defenderInventory)
    {
        var damage = attackerWeapon.GenerateDamage();

        if ((UnitIdType)defenderUnit.Id == UnitIdType.TurtleMan) 
            return damage - (int)defenderUnit.Rules["cut_enemy_damage"];
        
        var bodyArmor = (Armor)GameData.Items[(int)ItemIdType.BodyArmor - 1];
        
        if (defenderInventory.Any(item => item.Id == bodyArmor.Id)) 
            damage -= bodyArmor.DamageReduction;

        return damage;
    }
    
    public static int CalculateDistance(Location attackerLocation, Location defenderLocation)
    { 
        const int baseDistance = (int)DistanceType.Same;
            
        if (attackerLocation == defenderLocation) 
            return baseDistance;
            
        var distance = (LocationIdType)attackerLocation.Id == LocationIdType.EagleCliff 
            ? baseDistance - attackerLocation.Rules["cut_distance_bonus"] 
            : baseDistance;
    
        if (attackerLocation.AdjacentLocations.Contains(defenderLocation.Id)) 
            return distance + (int)DistanceType.Adjacent;
            
        if (attackerLocation.DistantLocations.Contains(defenderLocation.Id)) 
            return distance + (int)DistanceType.Distant;
    
        return distance + (int)DistanceType.Unavailable;
    }
    
    public static int CalculateAccuracy(
        Weapon attackerWeapon, 
        Unit attackerUnit, 
        List<Item> attackerInventory,
        Unit defenderUnit,
        List<Item> defenderInventory,
        int distance
    )
    {
        var laserSight = (WeaponUpgrade)GameData.Items[(int)ItemIdType.LaserSight - 1];
        var camouflageCloak = (Camouflage)GameData.Items[(int)ItemIdType.CamouflageCloak - 1];

        var attackerHasLaserSight = attackerInventory
            .Any(item => item.Id == (int)ItemIdType.LaserSight);
        var defenderHasCamouflageCloak = defenderInventory
            .Any(item => item.Id == (int)ItemIdType.CamouflageCloak);
        
        var basicAccuracy = attackerWeapon.Accuracy - distance;
        
        var accuracy = (UnitIdType)defenderUnit.Id == UnitIdType.TurtleMan || defenderHasCamouflageCloak
            ? basicAccuracy - camouflageCloak.CutEnemyAccuracyBonus // эффект панциря эквивалентен.
            : basicAccuracy;

        if ((UnitIdType)attackerUnit.Id == UnitIdType.Gunfighter) 
            accuracy += (int)attackerUnit.Rules["accuracy_bonus"];

        if (attackerHasLaserSight)
            accuracy += laserSight.AccuracyBonus;

        return accuracy;
    }
    
    private static int CalculateMinRequiredDiceValue(int accuracy)
    {
        return (int)DiceValueType.Six - accuracy + 1;
    }
    
    public static bool AttackWasSuccessful(int accuracy)
    {
        var minRequiredDiceValue = CalculateMinRequiredDiceValue(accuracy);
        var playerDiceValue = new Random().Next((int)DiceValueType.One, (int)DiceValueType.Six + 1);
        return playerDiceValue >= minRequiredDiceValue;
    }

    public static bool PlayersCanHandfight(Player attacker, Player defender)
    {
        return attacker.Location == defender.Location;
    }

    public static bool PlayersCanGunFight(Weapon weapon, int distance)
    {
        return weapon.Distance >= distance;
    }
    
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
        var number = new Random().Next(minPercent, maxPercent + 1);

        gotInjury = number <= injuryProbability;
        if (!gotInjury)
        {
            damageOfInjury = 0;
            attackerDied = false;
            return;
        }
        
        const int minDamage = 10;
        const int maxDamage = 30;
        
        damageOfInjury = new Random().Next(minDamage, maxDamage + 1);
        
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
            var damage = new Random().Next(minDamage, maxDamage + 1);
            
            player.Unit.TakeDamage(damage);
            
            var message = $"Игрок {player.UserName} получил {damage} урона от осколочного ранения."
                + (player.Unit.IsAlive() 
                    ? "" 
                    : " Игрок умер от последствий.");
            
            messagesForHitPlayers.Add(message);
        }
    }
}