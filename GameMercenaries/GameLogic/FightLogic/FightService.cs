using GameMercenaries.Constants;

namespace GameMercenaries.GameLogic.FightLogic;

using Models;
using Models.Items;
using static FightHelpers;
using static SideEffectsLogic;


public static class FightService
{
    public static HandFightResult HandFight(
        Player attacker, 
        Player defender,
        int dayNumber,
        Action<GameEvent> onGameEvent)
    {
        if (!PlayersCanHandfight(attacker, defender)) 
        {
            return new HandFightResult
            {
                PlayersCanFight = false,
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
                PlayersCanFight = true,
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

        GameEvent newGameEvent;
        GameEvent newPlayerGameEvent;
        
        if (!defender.Unit.IsAlive())
        {
            newGameEvent = new GameEvent
            {
                Day = dayNumber,
                Type = EventType.Death,
                Message = $"Игрок {defender.UserName} был убит игроком {attacker.UserName} в рукопашном бою!"
            };

            newPlayerGameEvent = new GameEvent
            {
                Day = dayNumber,
                Type = EventType.Death,
                Message = $"Вас убил игрок {attacker.UserName} в рукопашном бою!"
            };

            onGameEvent(newGameEvent);
            defender.AddEvent(newPlayerGameEvent);
            
            return new HandFightResult
            {
                PlayersCanFight = true,
                WasSuccessful = true,
                DamageDealt = damage,
                DefenderDied = true,
                DefenderCurrentHealth = 0,
                DefenderMaxHealth = defender.Unit.MaxHealth,
                Message = $"Вы убили игрока {defender.UserName}!"
            };
        }
        
        newGameEvent = new GameEvent
        {
            Day = dayNumber,
            Type = EventType.Attack,
            Message = $"Игрок {defender.UserName} был атакован игроком {attacker.UserName} в рукопашную и потерял {damage} жизней!"
        };

        newPlayerGameEvent = new GameEvent
        {
            Day = dayNumber,
            Type = EventType.Attack,
            Message = $"Вас атаковал игрок {attacker.UserName} в рукопашном бою! Вы потеряли {damage} жизней!"
        };

        onGameEvent(newGameEvent);
        defender.AddEvent(newPlayerGameEvent);
        
        return new HandFightResult
        {
            PlayersCanFight = true,
            WasSuccessful = true,
            DamageDealt = damage,
            DefenderDied = false,
            DefenderCurrentHealth = defender.Unit.CurrentHealth,
            DefenderMaxHealth = defender.Unit.MaxHealth,
            Message = $"Вы успешно атаковали игрока {defender.UserName}!"
        };
    }

    public static GunFightResult GunFight(
        Player attacker, 
        Player defender, 
        Weapon weapon,
        int dayNumber,
        Action<GameEvent> onGameEvent
        )
    {
        var distance = CalculateDistance(attacker.Location, defender.Location);
        
        if (!PlayersCanGunFight(weapon, distance))
        {
            return new GunFightResult
            {
                PlayersCanFight = false,
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
                PlayersCanFight = true,
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

        GameEvent newGameEvent;
        GameEvent newPlayerGameEvent;
        
        if (!defender.Unit.IsAlive())
        {
            newGameEvent = new GameEvent
            {
                Day = dayNumber,
                Type = EventType.Death,
                Message = $"Игрок {defender.UserName} был убит игроком {attacker.UserName} из оружия {weapon.Name}!"
            };

            newPlayerGameEvent = new GameEvent
            {
                Day = dayNumber,
                Type = EventType.Death,
                Message = $"Вас убил игрок {attacker.UserName} из оружия {weapon.Name}!"
            };
            
            onGameEvent(newGameEvent);
            defender.AddEvent(newPlayerGameEvent);
            
            return new GunFightResult
            {
                PlayersCanFight = true,
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
        
        newGameEvent = new GameEvent
        {
            Day = dayNumber,
            Type = EventType.Attack,
            Message = $"Игрок {defender.UserName} был атакован игроком {attacker.UserName} из оружия {weapon.Name} и потерял {damage} жизней!"
        };

        newPlayerGameEvent = new GameEvent
        {
            Day = dayNumber,
            Type = EventType.Attack,
            Message = $"Вас атаковал игрок {attacker.UserName} из оружия {weapon.Name}! Вы потеряли {damage} жизней!"
        };
        
        onGameEvent(newGameEvent);
        defender.AddEvent(newPlayerGameEvent);
        
        return new GunFightResult
        {
            PlayersCanFight = true,
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