using GameMercenaries.Constants;
using GameMercenaries.gameManagement;
using GameMercenaries.Models;
using GameMercenaries.Models.Items;

namespace GameMercenaries.GameLogic.FightLogic;

public static class FightHelpers
{
    private static readonly Random Random = new Random();

    public static int CalculateHandDamage(Unit attackerUnit, List<Item> inventory)
    {
        const int minDamage = 15;
        const int maxDamage = 30;
        const string handAttackBonus = "hand_attack_bonus";
        
        var baseDamage = Random.Next(minDamage, maxDamage + 1);
        var unitId = (UnitIdType)attackerUnit.Id;

        if (unitId == UnitIdType.CrabMan) 
            return baseDamage + Convert.ToInt32(attackerUnit.Rules[handAttackBonus]);

        var damage = baseDamage;

        var knife = (Weapon)GameData.Items[(int)ItemIdType.Knife - 1];

        if (inventory.Any(item => item.Id == knife.Id)) 
            damage = knife.GenerateDamage();
        
        if (unitId == UnitIdType.Brawler) 
            damage += Convert.ToInt32(attackerUnit.Rules[handAttackBonus]);

        return damage;
    }

    public static int CalculateWeaponDamage(Weapon attackerWeapon, Unit defenderUnit, List<Item> defenderInventory)
    {
        var damage = attackerWeapon.GenerateDamage();

        if ((UnitIdType)defenderUnit.Id == UnitIdType.TurtleMan) 
            return damage - Convert.ToInt32(defenderUnit.Rules["cut_enemy_damage"]);
        
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
            ? baseDistance - Convert.ToInt32(attackerLocation.Rules["cut_distance_bonus"]) 
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
            ? basicAccuracy - camouflageCloak.CutEnemyAccuracyBonus
            : basicAccuracy;

        if ((UnitIdType)attackerUnit.Id == UnitIdType.Gunfighter) 
            accuracy += Convert.ToInt32(attackerUnit.Rules["accuracy_bonus"]);

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
        var playerDiceValue = Random.Next((int)DiceValueType.One, (int)DiceValueType.Six + 1);
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
}
