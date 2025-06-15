namespace GameMercenaries.GameLogic.FightLogic;

using Models;
using Models.Items;
using static FightHelpers;
using static SideEffectsLogic;


public static class FightService
{
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
        
        defender.Unit.TakeDamage(damage);
        
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
                SideEffectMessage = null,
                EquipmentBreakMessages = []
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
        
        bool hit = AttackWasSuccessful(accuracy);
        
        var damage = CalculateWeaponDamage(weapon, defender.Unit, defender.Inventory);

        TwoPistolsLogic(
            attacker, 
            weapon, 
            accuracy, 
            out bool weaponIsPistol, 
            out int totalDamage
            );

        if (weaponIsPistol)
        {
            hit = totalDamage > 0;
            damage = totalDamage;
        }
        
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
                    : null,
                EquipmentBreakMessages = []
            };
        }

        defender.Unit.TakeDamage(damage);
        
        GrenadeLauncherLogic( 
            attacker,
            defender,
            weapon,
            out List<Player> hitByRpgPlayers,
            out List<string> messages);

        var equipmentBreakMessages = BreakEquipmentLogic(defender);

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
                HitByRpgPlayers = hitByRpgPlayers,
                MessagesForHitPlayers = messages,
                AttackerGotInjuredAtFactory = gotInjury,
                DamageOfInjury = damageOfInjury,
                AttackerDiedByInjury = attackerDied,
                SideEffectMessage = gotInjury 
                    ? $"Вы отравились на заводе и получили {damageOfInjury} урона." +
                      (attackerDied ? " Вы умерли от последствий!" : "")
                    : null,
                EquipmentBreakMessages = equipmentBreakMessages
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
            HitByRpgPlayers = hitByRpgPlayers,
            MessagesForHitPlayers = messages,
            AttackerGotInjuredAtFactory = gotInjury,
            DamageOfInjury = damageOfInjury,
            AttackerDiedByInjury = attackerDied,
            SideEffectMessage = gotInjury 
                ? $"Вы отравились на заводе и получили {damageOfInjury} урона." +
                  (attackerDied ? " Вы умерли от последствий!" : "")
                : null,
            EquipmentBreakMessages = equipmentBreakMessages
        };
    }
}