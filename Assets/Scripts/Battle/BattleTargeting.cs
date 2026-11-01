using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Battle
{
    public static class BattleTargeting
    {
        public static List<BattleUnit> GetEffectedCandidates(BattleUnit actor, SkillData skill, IReadOnlyList<BattleUnit> all)
        {
            switch (skill.Target)
            {
                case TargetType.Self:
                    return new List<BattleUnit> { actor };
                case TargetType.SingleAlly:
                    return all.Where(u => u.Team == actor.Team && !u.IsDead).ToList();
                default:
                    return all.Where(u => u.Team != actor.Team && !u.IsDead).ToList();
            }
        }


        public static bool IsNeedPickingTarget(SkillData skill, int remainingCandidateCount)
        {
            bool single = skill.Target == TargetType.SingleAlly || skill.Target == TargetType.SingleEnemy;
            return single && remainingCandidateCount > 1;
        }


        public static bool IsSupportSkill(SkillData skill)
        {
            return skill.Target == TargetType.Self || skill.Target == TargetType.SingleAlly;
        }


        public static List<BattleUnit> HandleCastSkillOnTarget(BattleUnit actor, SkillData skill, BattleUnit picked, IReadOnlyList<BattleUnit> all)
        {
            var candidattes = GetEffectedCandidates(actor, skill, all);
            if (skill.Target == TargetType.AllEnemies || skill.Target == TargetType.Self)
                return candidattes;

            if (picked != null && !picked.IsDead && candidattes.Contains(picked))
                return new List<BattleUnit> { picked };

            return candidattes.Take(1).ToList();
        }
    }
}
