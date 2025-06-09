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

    private static bool PlayersCanGunFight(Weapon weapon, int distance)
    {
        return weapon.Distance >= distance;
    }
    
    public static HandFightResult HandFight(Player attacker, Player defender)
    {
        if (!PlayersCanHandfight(attacker, defender)) 
        {
            return new HandFightResult
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
            return new HandFightResult
            {
                WasSuccessful = false,
                DamageDealt = 0,
                DefenderDied = false,
                DefenderCurrentHealth = defender.Unit.CurrentHealth,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Игрок {defender.UserName} увернулся от вашей атаки!"
            };
        }

        var damage = CalculateHandDamage(attacker.Unit, attacker.Inventory);
        
        if (!defender.Unit.IsAlive())
        {
            return new HandFightResult
            {
                WasSuccessful = true,
                DamageDealt = damage,
                DefenderDied = true,
                DefenderCurrentHealth = 0,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Вы убили игрока {defender.UserName}!"
            };
        }

        return new HandFightResult
        {
            WasSuccessful = true,
            DamageDealt = damage,
            DefenderDied = false,
            DefenderCurrentHealth = defender.Unit.CurrentHealth,
            DefenderMaxHealth = defender.Unit.MaxHealth,
            Message = $"Вы успешно атаковали игрока {defender.UserName}!"
        };
    }

    private static void InjuryLogic(
        Player attacker, 
        bool attackWasSuccessful,
        out bool gotInjury,
        out int damageOfInjury,
        out bool attackerDied)
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

    public static GunFightResult GunFight(Player attacker, Player defender, Weapon weapon)
    {
        var distance = CalculateDistance(attacker.Location, defender.Location);
        
        if (!PlayersCanGunFight(weapon, distance))
        {
            return new GunFightResult
            {
                WasSuccessful = false,
                DamageDealt = 0,
                DefenderDied = false,
                DefenderCurrentHealth = defender.Unit.CurrentHealth,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Дальность стрельбы вашего оружия не позволяет атаковать этого игрока!",
                HitByRpgPlayers = [],
                MessagesForHitPlayers = [],
                AttackerGotInjuredAtFactory = false,
                DamageOfInjury = 0,
                AttackerDiedByInjury = false,
                SideEffectMessage = null
            };
        }

        var accuracy = CalculateAccuracy(
            weapon,
            attacker.Unit,
            attacker.Inventory,
            defender.Unit,
            defender.Inventory,
            distance
        );

        var damage = CalculateWeaponDamage(weapon, defender.Unit, defender.Inventory);

        bool hit = AttackWasSuccessful(accuracy);
        
        InjuryLogic(
            attacker, 
            hit,
            out bool gotInjury, 
            out int damageOfInjury, 
            out bool attackerDied);

        if (!hit)
        {
            return new GunFightResult
            {
                WasSuccessful = false,
                DamageDealt = 0,
                DefenderDied = false,
                DefenderCurrentHealth = defender.Unit.CurrentHealth,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Вы не попали в игрока {defender.UserName}!",
                HitByRpgPlayers = [],
                MessagesForHitPlayers = [],
                AttackerGotInjuredAtFactory = gotInjury,
                DamageOfInjury = damageOfInjury,
                AttackerDiedByInjury = attackerDied,
                SideEffectMessage = gotInjury 
                    ? $"Вы отравились на заводе и получили {damageOfInjury} урона." +
                      (attackerDied ? " Вы умерли от последствий!" : "")
                    : null
            };
        }

        if (!defender.Unit.IsAlive())
        {
            return new GunFightResult
            {
                WasSuccessful = true,
                DamageDealt = damage,
                DefenderDied = true,
                DefenderCurrentHealth = 0,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Вы убили игрока {defender.UserName}!",
                HitByRpgPlayers = [],
                MessagesForHitPlayers = [],
                AttackerGotInjuredAtFactory = gotInjury,
                DamageOfInjury = damageOfInjury,
                AttackerDiedByInjury = attackerDied,
                SideEffectMessage = gotInjury 
                    ? $"Вы отравились на заводе и получили {damageOfInjury} урона." +
                      (attackerDied ? " Вы умерли от последствий!" : "")
                    : null
            };
        }
        
        return new GunFightResult
        {
            WasSuccessful = true,
            DamageDealt = damage,
            DefenderDied = false,
            DefenderCurrentHealth = defender.Unit.CurrentHealth,
            DefenderMaxHealth = defender.Unit.MaxHealth,
            Message = $"Вы успешно атаковали игрока {defender.UserName}!",
            HitByRpgPlayers = [],
            MessagesForHitPlayers = [],
            AttackerGotInjuredAtFactory = gotInjury,
            DamageOfInjury = damageOfInjury,
            AttackerDiedByInjury = attackerDied,
            SideEffectMessage = gotInjury 
                ? $"Вы отравились на заводе и получили {damageOfInjury} урона." +
                  (attackerDied ? " Вы умерли от последствий!" : "")
                : null
        };
    }
}