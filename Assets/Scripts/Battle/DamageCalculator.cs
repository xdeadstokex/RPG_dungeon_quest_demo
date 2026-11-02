using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class DamageCalculator
    {
        private const float PERFECT_ATTACK = 1.5f;
        private const float GOOD_ATTACK = 1.0f;
        private const float MISS_ATTACK = 0.0f;
        private const float PERFECT_PARRY = 0.0f;
        private const float MISS_PARRY = 1.0f;


        public static float AttackScale(CommandRanting rating)
        {
            switch (rating)
            {
                case CommandRanting.Perfect:
                    return PERFECT_ATTACK;
                case CommandRanting.Good:
                    return GOOD_ATTACK;
                case CommandRanting.Miss:
                    return MISS_ATTACK;
                default:
                    return PERFECT_ATTACK;
            }
        }


        public static float DefendScale(CommandRanting rating)
        {
            switch (rating)
            {
                case CommandRanting.Perfect:
                    return PERFECT_PARRY;
                case CommandRanting.Miss:
                    return MISS_PARRY;
                default:
                    return MISS_PARRY;
            }
        }


        public static int Raw(BattleUnit attacker, SkillData skill, float commandScale, out bool crit)
        {
            int baseDmg = attacker.RollAttackDamage(out crit);
            return Mathf.RoundToInt((baseDmg + skill.Power) * commandScale); //maybe change a little after
        }
    }
}
