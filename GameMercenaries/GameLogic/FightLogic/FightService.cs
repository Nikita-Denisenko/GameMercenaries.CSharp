namespace GameMercenaries.GameLogic.FightLogic;

using Constants;
using gameManagement;
using Models;
using Models.Items;


public static class FightService
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

    private static int CalculateDistance(Location attackerLocation, Location defenderLocation)
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

    public static int CalculateMinRequiredDiceValue(int accuracy)
    {
        return (int)DiceValueType.Six - accuracy + 1;
    }
    
    private static bool AttackWasSuccessful(int accuracy)
    {
        var minRequiredDiceValue = CalculateMinRequiredDiceValue(accuracy);
        var playerDiceValue = new Random().Next((int)DiceValueType.One, (int)DiceValueType.Six + 1);
        return playerDiceValue >= minRequiredDiceValue;
    }

    private static bool PlayersCanHandfight(Player attacker, Player defender)
    {
        return attacker.Location == defender.Location;
    }
    
    public static FightResult HandFight(Player attacker, Player defender)
    {
        if (!PlayersCanHandfight(attacker, defender)) 
        {
            return new FightResult
            {
                WasSuccessful = false,
                DamageDealt = 0,
                DefenderDied = false,
                DefenderCurrentHealth = defender.Unit.CurrentHealth,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = "Цель находится в другой локации. Рукопашная атака невозможна."
            };
        }

        const int handFightAccuracy = 4;
        bool hit = AttackWasSuccessful(handFightAccuracy);

        if (!hit)
        {
            return new FightResult
            {
                WasSuccessful = false,
                DamageDealt = 0,
                DefenderDied = false,
                DefenderCurrentHealth = defender.Unit.CurrentHealth,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Игрок {attacker.UserName} увернулся от вашей атаки!"
            };
        }

        var damage = CalculateHandDamage(attacker.Unit, attacker.Inventory);
        
        if (!defender.Unit.IsAlive())
        {
            return new FightResult
            {
                WasSuccessful = true,
                DamageDealt = damage,
                DefenderDied = true,
                DefenderCurrentHealth = 0,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Вы убили игрока {attacker.UserName}!"
            };
        }

        return new FightResult
        {
            WasSuccessful = true,
            DamageDealt = damage,
            DefenderDied = false,
            DefenderCurrentHealth = defender.Unit.CurrentHealth,
            DefenderMaxHealth = defender.Unit.MaxHealth,
            Message = $"Вы успешно атаковали игрока {defender.UserName}!"
        };
    }
}