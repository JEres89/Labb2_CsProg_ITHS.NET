using Labb2_CsProg_ITHS.NET.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Game;
internal static class CombatProvider
{

    internal record CombatResult
    {
        public readonly int attackRoll;
        public readonly int defenseRoll;

        public readonly int damage;

        public CombatResult(int attackRoll, int defenseRoll, int damage)
        {
            this.attackRoll = attackRoll;
            this.defenseRoll = defenseRoll;
            this.damage = damage;
        }

        public string GenerateCombatMessage(LevelEntity attacker, LevelEntity defender)
        {
            string msg = $"{attacker.Name} attacks {defender.Name} with a roll of {attacker.AttackDieNum}d{attacker.AttackDieSize}+{attacker.AttackMod} = {attackRoll} vs {defender.DefenseDieNum}d{defender.DefenseDieSize}+{defender.DefenseMod} = {defenseRoll}";
            return msg;
        }
    }
    internal static CombatResult Attack(LevelEntity attacker, LevelEntity defender)
    {
        int attackRoll = Dice.Roll(attacker.AttackDieNum, attacker.AttackDieSize) + attacker.AttackMod;
        int defenseRoll = Dice.Roll(defender.DefenseDieNum, defender.DefenseDieSize) + defender.DefenseMod;

        int damage = attackRoll > defenseRoll ? attackRoll - defenseRoll : 0;
        return new(attackRoll, defenseRoll, damage);
    }

    //internal static void Heal(LevelEntity healer, LevelEntity target)
    //{

    //}

    //internal static void ApplyDamageOverTime(LevelEntity entity, int damage)
    //{
    //	entity.Health -= damage;
    //}

    //internal static void ApplyHealingOverTime(LevelEntity entity, int healing)
    //{
    //	entity.Health += healing;
    //}
}
