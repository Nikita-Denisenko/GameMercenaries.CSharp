using GameMercenaries.Constants;
using GameMercenaries.gameManagement;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;

namespace GameMercenaries.GameLogic;

public static class FightLogic
{
    public static int CalculateHandDamage(Unit unit, List<Item> inventory)
    {
        const int minDamage = 15;
        const int maxDamage = 30;
        const string handAttackBonus = "hand_attack_bonus";
        
        var baseDamage = new Random().Next(minDamage, maxDamage + 1);
        var unitId = (UnitId)unit.Id;

        if (unitId == UnitId.CrabMan) 
            return baseDamage + (int)unit.Rules[handAttackBonus];

        var damage = baseDamage;

        var knife = (Weapon)GameData.Items[(int)ItemId.Knife - 1];

        if (inventory.Any(item => item.Id == knife.Id)) 
            damage = knife.GenerateDamage();
        
        if (unitId == UnitId.Brawler) 
            damage += (int)unit.Rules[handAttackBonus];

        return damage;
    }
    
    public static int CalculateWeaponDamage(Weapon attackerWeapon, Unit defenderUnit, List<Item> defenderInventory)
    {
        var damage = attackerWeapon.GenerateDamage();

        if ((UnitId)defenderUnit.Id == UnitId.TurtleMan) 
            return damage - (int)defenderUnit.Rules["cut_enemy_damage"];
        
        var bodyArmor = (Armor)GameData.Items[(int)ItemId.BodyArmor - 1];
        
        if (defenderInventory.Any(item => item.Id == bodyArmor.Id)) 
            damage -= bodyArmor.DamageReduction;

        return damage;
    }

    public static int CalculateDistance(Location attackerLocation, Location defenderLocation)
    {
        const int baseDistance = (int)DistanceType.Same;
        
        if (attackerLocation == defenderLocation) 
            return baseDistance;
        
        var distance = (LocationId)attackerLocation.Id == LocationId.EagleCliff 
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
        var laserSight = (WeaponUpgrade)GameData.Items[(int)ItemId.LaserSight - 1];
        var camouflageCloak = (Camouflage)GameData.Items[(int)ItemId.CamouflageCloak - 1];

        var attackerHasLaserSight = attackerInventory
            .Any(item => item.Id == (int)ItemId.LaserSight);
        var defenderHasCamouflageCloak = defenderInventory
            .Any(item => item.Id == (int)ItemId.CamouflageCloak);
        
        var basicAccuracy = attackerWeapon.Accuracy - distance;
        
        var accuracy = (UnitId)defenderUnit.Id == UnitId.TurtleMan || defenderHasCamouflageCloak
            ? basicAccuracy - camouflageCloak.CutEnemyAccuracyBonus // эффект панциря эквивалентен.
            : basicAccuracy;

        if ((UnitId)attackerUnit.Id == UnitId.Gunfighter) 
            accuracy += (int)attackerUnit.Rules["accuracy_bonus"];

        if (attackerHasLaserSight)
            accuracy += laserSight.AccuracyBonus;

        return accuracy;
    }
}